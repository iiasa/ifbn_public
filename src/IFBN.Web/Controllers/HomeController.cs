// <copyright file="HomeController.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Mime;
	using System.Security.Claims;
	using IFBN.Data;
	using IFBN.Web.Core;
	using IFBN.Web.Models;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Newtonsoft.Json;
	using GeoJSON.Net.Feature;
	using IFBN.Data.Entities;
	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
	using WebApiContrib.Core.Formatter.Csv;
    //using System.Xml;
    using System.Data;
    using System.Text;
    using ChoETL;
	using System.IO;
	using System.Text.Json;


	public class HomeController : Controller
	{
		private static Lazy<ICollection<Site>> sites = new Lazy<ICollection<Site>>(GetPublicSites);

		private static Lazy<ICollection<Site>> sitesConfidential = new Lazy<ICollection<Site>>(GetConfidentialSites);

		private readonly ILogger<HomeController> log;

		private readonly IHttpContextAccessor httpContextAccessor;

		private readonly IFBNContext context;

		public HomeController(ILogger<HomeController> log, IHttpContextAccessor httpContextAccessor, IFBNContext context)
		{
			this.log = log;
			this.httpContextAccessor = httpContextAccessor;
			this.context = context;
		}

		[HttpGet]
		[Authorize]
		[Route("Download/{country}/{intendedUse}")]
		public IActionResult Download(string country, string intendedUse)
		{

			string downloadtype = "geojson";
			IEnumerable<Site> sites = GetSites(country);
			List<GeoJSON.Net.Feature.Feature> features = new List<GeoJSON.Net.Feature.Feature>();
			sites.ToList().ForEach(x => features.Add(new GeoJSON.Net.Feature.Feature(GeoJsonHelper.WktStringToGeometry(x.Geometry), new GeoJsonProperties(x.Id, x.Name, x.Country, x))));

			string sitesString = JsonConvert.SerializeObject(new FeatureCollection(features));

			// add two links to the downloaded file:
			// http://forest-observation-system.net/
			// https://www.nature.com/articles/s41597-019-0196-1
			string siteLinks = "\n" + "http://forest-observation-system.net/" + "\n" +
				"https://www.nature.com/articles/s41597-019-0196-1";
			sitesString = sitesString + siteLinks;

			string filename = "FOS-" + country + DateTime.Now.Ticks + "." + downloadtype;

			ContentDisposition cd = new ContentDisposition
			{
				FileName = filename,

				// Always prompt the user for downloading, set to true if you want the browser to try to show the file inline
				Inline = false,
			};
			Response.Headers.Add("Content-Disposition", cd.ToString());
			TrackDownload(filename, intendedUse);
			return File(Helpers.GetBytes(sitesString), "application/json");

		}

		[HttpGet]
		[Authorize]
		//[Route("datacomplex.csv")]
		[Route("DownloadCSV/{country}/{intendedUse}")]
		[Produces("text/csv")]
		public IActionResult DownloadCSV(string country, string intendedUse)
		{
			string downloadtype = "csv";
			IEnumerable<Site> sites = GetSites(country);
			List<GeoJSON.Net.Feature.Feature> features = new List<GeoJSON.Net.Feature.Feature>();
			sites.ToList().ForEach(x => features.Add(new GeoJSON.Net.Feature.Feature(GeoJsonHelper.WktStringToGeometry(x.Geometry), new GeoJsonProperties(x.Id, x.Name, x.Country, x))));

			string sitesString = JsonConvert.SerializeObject(new FeatureCollection(features));

			StringBuilder csvFeatures = new StringBuilder();
			//features

			// craete csv for getUsers node  
			using (var feature = ChoJSONReader.LoadText(sitesString)
					.WithJSONPath("$..features[*]", true)
					.Configure(c => c.JsonSerializerSettings = new JsonSerializerSettings
					{
						DateParseHandling = DateParseHandling.None,
						DateTimeZoneHandling = DateTimeZoneHandling.Utc,
						Formatting = Formatting.Indented,

					})
					)
			{
				// to get users count  
				var arrFeatures = feature.ToArray();
				int featuresCount = arrFeatures.Length;

				using (var w = new ChoCSVWriter(csvFeatures)
					.WithFirstLineHeader()
					.Configure(c => c.MaxScanRows = featuresCount)
					.Configure(c => c.ThrowAndStopOnMissingField = false)
					)
				{
					w.Write(arrFeatures);
				}
			}

			char separator = ',';
			// Split csv by new line  
			var objFeatures = csvFeatures.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
			//// remove newline of inside string  
			//var contents = Regex.Replace(csvErrors.ToString(), "\"[^\"]*(?:\"\"[^\"]*)*\"", m => m.Value.Replace("\n", "").Replace("\r", ""));
			string defaultValue = string.Empty;
			int cnt = 0;
			cnt = objFeatures.FirstOrDefault()?.Split(separator).Length ?? 0;


			if (cnt > 0)
				defaultValue = defaultValue.PadLeft(cnt - 1, separator);
			// set both csv rows are equal, merge only work when rows are equal  
			var fullString = objFeatures;//.Merge(objErrors, defaultValue, (f, s) => string.Join(separator.ToString(), f, s)).ToArray();
										 //File.AppendAllLines("folderpath/OutputSample.csv", fullString);

			//add two links to the downloaded file:
			//http://forest-observation-system.net/
			//https://www.nature.com/articles/s41597-019-0196-1
			string siteLinks = "\n" + "http://forest-observation-system.net/" + "\n" +
				"https://www.nature.com/articles/s41597-019-0196-1";
			//sitesString = sitesString + siteLinks;
			sitesString = string.Join('\n', fullString);
			sitesString = sitesString + siteLinks;
			string filename = "FOS-" + country + "_" + DateTime.Now.Ticks + "." + downloadtype;
			ContentDisposition cd = new ContentDisposition
			{
				FileName = filename,

				// Always prompt the user for downloading, set to true if you want
				// the browser to try to show the file inline
				Inline = false,
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			TrackDownload(filename, intendedUse);
			return File(Helpers.GetBytes(sitesString), "text/csv");
		}

		[HttpGet]
		[Authorize]
		//[Route("datacomplex.csv")]
		[Route("DownloadCSV_old/{country}/{intendedUse}")]
		[Produces("text/csv")]
		public IActionResult DownloadCSV_old(string country, string intendedUse)
		{
			string downloadtype = "csv";
			string filename = "FOS-" + country + DateTime.Now.Ticks + "." + downloadtype;
			ContentDisposition cd = new ContentDisposition
			{
				FileName = filename,

				// Always prompt the user for downloading, set to true if you want the browser to try to show the file inline
				Inline = false,
			};
			Response.Headers.Add("Content-Disposition", cd.ToString());
			TrackDownload(filename, intendedUse);

			//add two links to the downloaded file:
			//http://forest-observation-system.net/
			//https://www.nature.com/articles/s41597-019-0196-1

			List<PlotExport> sites = GetSitesExport(country).ToList();
			PlotExport links = new PlotExport();
			links.SiteName = "http://forest-observation-system.net/";
			sites.Add(links);
			links = new PlotExport();
			links.SiteName = "https://www.nature.com/articles/s41597-019-0196-1";
			sites.Add(links);
			return Ok(sites);
			//return Ok(Helpers.Plotexport(GetSites(country)));
		}

		[HttpGet]
		[Authorize]
		[Route("DownloadBBox/{bbox}/{intendedUse}")] //BBOX={west},{south},{east},{north} e.g. BBOX=-180,-90,180,90
		public IActionResult DownloadBBox(string bbox, string intendedUse)
		{
			string downloadtype = "geojson";
			bbox = bbox.Substring(bbox.IndexOf("=") + 1);
			string[] latlon = bbox.Split(","); //{west},{south},{east},{north} => x1, y1, x2, y2

			double longitudeWest = double.Parse(latlon[0].ToString());
			double lattitudeSouth = double.Parse(latlon[1].ToString());
			double longitudeEast = double.Parse(latlon[2].ToString());
			double lattitudeNorth = double.Parse(latlon[3].ToString());

			//List<Site> sites = new List<Site>();

			//foreach (var site in HomeController.sites.Value)
			//{
			//	if (site.Geometry != null)
			//	{
			//		if (Helpers.WithinRectangle(lattitudeNorth, longitudeWest, lattitudeSouth, longitudeEast,
			//			GeoJsonHelper.WktStringToPoint(site.Geometry).Coordinates.Latitude,
			//			GeoJsonHelper.WktStringToPoint(site.Geometry).Coordinates.Longitude))
			//		{
			//			sites.Add(site);
			//		}
			//	}
			//}

			//string coordinateWkt = "POINT(15 20)"; //--> x/lon y/lat
			//coordinateWkt = "POLYGON ((-83.53656 8.705206, -83.53656 8.705696, -83.536056 8.705696, -83.536056 8.705206, -83.53656 8.705206))";
			//GeoJSON.Net.Geometry.Point geom = GeoJsonHelper.WktStringToPoint(coordinateWkt);

			//string wkttest = "POLYGON((-83.524725 8.70698, -83.524725 8.708402, -83.524645 8.708402, -83.524645 8.70698, -83.524725 8.70698))";
			//wkttest = "POLYGON((-83.524725 8.70698, -83.524725 8.708402, -83.524645 8.708402, -83.524645 8.70698, -83.524725 8.70698))";
			//GeoJsonHelper.WktStringToPoly(wkttest);


			//IEnumerable<Site> sites = HomeController.sites.Value.Where(x => x.Geometry!=null && Helpers.WithinRectangle(lattitudeNorth, longitudeWest, lattitudeSouth, longitudeEast, GeoJsonHelper.WktStringToPoint(x.Geometry).Coordinates.Latitude, GeoJsonHelper.WktStringToPoint(x.Geometry).Coordinates.Longitude));

			IEnumerable<Site> sites = GetSites(longitudeWest, lattitudeSouth, longitudeEast, lattitudeNorth);
			List<GeoJSON.Net.Feature.Feature> features = new List<GeoJSON.Net.Feature.Feature>();

			sites.ToList().ForEach(x => features.Add(new GeoJSON.Net.Feature.Feature(GeoJsonHelper.WktStringToGeometry(x.Geometry), new GeoJsonProperties(x.Id, x.Name, x.Country, x))));

			string sitesString = JsonConvert.SerializeObject(new FeatureCollection(features));

			//add two links to the downloaded file:
			//http://forest-observation-system.net/
			//https://www.nature.com/articles/s41597-019-0196-1
			string siteLinks = "\n" + "http://forest-observation-system.net/" + "\n" +
				"https://www.nature.com/articles/s41597-019-0196-1";
			sitesString = sitesString + siteLinks;
			string filename = "FOS-" + bbox + "_" + DateTime.Now.Ticks + "." + downloadtype;
			ContentDisposition cd = new ContentDisposition
			{
				FileName = filename,

				// Always prompt the user for downloading, set to true if you want
				// the browser to try to show the file inline
				Inline = false,
			};
			Response.Headers.Add("Content-Disposition", cd.ToString());
			TrackDownload(filename, intendedUse);
			return File(Helpers.GetBytes(sitesString), "application/json");
		}

		[HttpGet]
		[Authorize]
		//[Route("datacomplex.csv")]
		[Route("DownloadBBoxCSV_old/{bbox}/{intendedUse}")]
		[Produces("text/csv")]
		public IActionResult DownloadBBoxCSV_old(string bbox, string intendedUse)
		{
			string downloadtype = "csv";
			bbox = bbox.Substring(bbox.IndexOf("=") + 1);
			string[] latlon = bbox.Split(","); //{west},{south},{east},{north} => x1, y1, x2, y2

			double longitudeWest = double.Parse(latlon[0].ToString());
			double lattitudeSouth = double.Parse(latlon[1].ToString());
			double longitudeEast = double.Parse(latlon[2].ToString());
			double lattitudeNorth = double.Parse(latlon[3].ToString());

			string filename = "FOS-" + bbox + DateTime.Now.Ticks + "." + downloadtype;
			ContentDisposition cd = new ContentDisposition
			{
				FileName = filename,
				// Always prompt the user for downloading, set to true if you want
				// the browser to try to show the file inline
				Inline = false,
			};
			Response.Headers.Add("Content-Disposition", cd.ToString());
			TrackDownload(filename, intendedUse);

			//add two links to the downloaded file:
			//http://forest-observation-system.net/
			//https://www.nature.com/articles/s41597-019-0196-1

			List<PlotExport> sites = GetSitesExport(longitudeWest, lattitudeSouth, longitudeEast, lattitudeNorth).ToList();
			PlotExport links = new PlotExport();
			links.SiteName = "http://forest-observation-system.net/";
			sites.Add(links);
			links = new PlotExport();
			links.SiteName = "https://www.nature.com/articles/s41597-019-0196-1";
			sites.Add(links);
			return Ok(sites);

			//return Ok(GetSitesExport(longitudeWest, lattitudeSouth, longitudeEast, lattitudeNorth));
		}

		[HttpGet]
		[Authorize]
		//[Route("datacomplex.csv")]
		[Route("DownloadBBoxCSV/{bbox}/{intendedUse}")]
		[Produces("text/csv")]
		public IActionResult DownloadBBoxCSV(string bbox, string intendedUse)
		{
			string downloadtype = "csv";
			bbox = bbox.Substring(bbox.IndexOf("=") + 1);
			string[] latlon = bbox.Split(","); //{west},{south},{east},{north} => x1, y1, x2, y2

			double longitudeWest = double.Parse(latlon[0].ToString());
			double lattitudeSouth = double.Parse(latlon[1].ToString());
			double longitudeEast = double.Parse(latlon[2].ToString());
			double lattitudeNorth = double.Parse(latlon[3].ToString());

			IEnumerable<Site> sites = GetSites(longitudeWest, lattitudeSouth, longitudeEast, lattitudeNorth);
			List<GeoJSON.Net.Feature.Feature> features = new List<GeoJSON.Net.Feature.Feature>();

			sites.ToList().ForEach(x => features.Add(new GeoJSON.Net.Feature.Feature(GeoJsonHelper.WktStringToGeometry(x.Geometry), new GeoJsonProperties(x.Id, x.Name, x.Country, x))));

			string sitesString =JsonConvert.SerializeObject(new FeatureCollection(features));

			StringBuilder csvFeatures = new StringBuilder();
			//features

			// craete csv for getUsers node  
			using (var feature = ChoJSONReader.LoadText(sitesString)
					.WithJSONPath("$..features[*]",true)
					.Configure(c => c.JsonSerializerSettings = new JsonSerializerSettings
					{
						DateParseHandling = DateParseHandling.None,
						DateTimeZoneHandling = DateTimeZoneHandling.Utc,
						Formatting = Formatting.Indented,
						
					})
					)
			{
				// to get users count  
				var arrFeatures = feature.ToArray();
				int featuresCount = arrFeatures.Length;

				using (var w = new ChoCSVWriter(csvFeatures)
					.WithFirstLineHeader()
					.Configure(c => c.MaxScanRows = featuresCount)
					.Configure(c => c.ThrowAndStopOnMissingField = false)
					)
				{
					w.Write(arrFeatures);
				}
			}

			char separator = ',';
			// Split csv by new line  
			var objFeatures = csvFeatures.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
			//// remove newline of inside string  
			//var contents = Regex.Replace(csvErrors.ToString(), "\"[^\"]*(?:\"\"[^\"]*)*\"", m => m.Value.Replace("\n", "").Replace("\r", ""));
			string defaultValue = string.Empty;
			int cnt = 0;
			cnt = objFeatures.FirstOrDefault()?.Split(separator).Length ?? 0;
		
			if (cnt > 0)
				defaultValue = defaultValue.PadLeft(cnt - 1, separator);
			// set both csv rows are equal, merge only work when rows are equal  
			var fullString = objFeatures;//.Merge(objErrors, defaultValue, (f, s) => string.Join(separator.ToString(), f, s)).ToArray();
			//File.AppendAllLines("folderpath/OutputSample.csv", fullString);


			//add two links to the downloaded file:
			//http://forest-observation-system.net/
			//https://www.nature.com/articles/s41597-019-0196-1
			string siteLinks = "\n" + "http://forest-observation-system.net/" + "\n" +
				"https://www.nature.com/articles/s41597-019-0196-1";
			//sitesString = sitesString + siteLinks;
			sitesString = string.Join('\n',fullString);
			sitesString = sitesString + siteLinks;
			string filename = "FOS-" + bbox + "_" + DateTime.Now.Ticks + "." + downloadtype;
			ContentDisposition cd = new ContentDisposition
			{
				FileName = filename,

				// Always prompt the user for downloading, set to true if you want
				// the browser to try to show the file inline
				Inline = false,
			};

			Response.Headers.Add("Content-Disposition", cd.ToString());
			TrackDownload(filename, intendedUse);
			return File(Helpers.GetBytes(sitesString), "text/csv");
		}

		[HttpGet]
		[Route("DownloadBBox_extern/{bbox}/{intendedUse}/{application}")]
		public IActionResult DownloadBBox_extern(string bbox, string intendedUse, string application)
		{
			string downloadtype = "geojson";
			bbox = bbox.Substring(bbox.IndexOf("=") + 1);
			string[] latlon = bbox.Split(","); //{west},{south},{east},{north} => x1, y1, x2, y2

			double longitudeWest = double.Parse(latlon[0].ToString());
			double lattitudeSouth = double.Parse(latlon[1].ToString());
			double longitudeEast = double.Parse(latlon[2].ToString());
			double lattitudeNorth = double.Parse(latlon[3].ToString());

			IEnumerable<Site> sites = GetSites(longitudeWest, lattitudeSouth, longitudeEast, lattitudeNorth);
			List<GeoJSON.Net.Feature.Feature> features = new List<GeoJSON.Net.Feature.Feature>();

			sites.ToList().ForEach(x => features.Add(new GeoJSON.Net.Feature.Feature(GeoJsonHelper.WktStringToGeometry(x.Geometry), new GeoJsonProperties(x.Id, x.Name, x.Country, x))));

			string sitesString = JsonConvert.SerializeObject(new FeatureCollection(features));

			//add two links to the downloaded file:
			//http://forest-observation-system.net/
			//https://www.nature.com/articles/s41597-019-0196-1
			//string siteLinks = "\n" + "http://forest-observation-system.net/" + "\n" +
			//	"https://www.nature.com/articles/s41597-019-0196-1";
			//sitesString = sitesString + siteLinks;
			string filename = "FOS-" + bbox + "_" + DateTime.Now.Ticks + "." + downloadtype;
			ContentDisposition cd = new ContentDisposition
			{
				FileName = filename,

				// Always prompt the user for downloading, set to true if you want
				// the browser to try to show the file inline
				Inline = false,
			};
			Response.Headers.Add("Content-Disposition", cd.ToString());
			TrackDownload(filename, intendedUse, application);
			return File(Helpers.GetBytes(sitesString), "application/json");
		}

		public void TrackDownload(string filename, string intendedUse, string application = "FOS_WebPortal")
		{
			var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

			this.context.Downloads.Add(new Download()
			{
				Timestamp = DateTime.Now,
				Filename = filename,
				IntendedUse = intendedUse,
				UserId = userId,
				Application = application,
			});

			this.context.SaveChanges();
		}

		public static IEnumerable<Site> GetSites(string country)
		{
			IEnumerable<Site> sites = HomeController.sites.Value.Where(x => x.Country == country);
			return sites;
		}

		public static IEnumerable<Site> GetSites(double longitudeWest, double lattitudeSouth, double longitudeEast, double lattitudeNorth)
		{
			IEnumerable<Site> sites = HomeController.sites.Value.Where(x => x.Geometry != null && Helpers.WithinRectangle(lattitudeNorth, longitudeWest, lattitudeSouth, longitudeEast, GeoJsonHelper.WktStringToPoint(x.Geometry).Coordinates.Latitude, GeoJsonHelper.WktStringToPoint(x.Geometry).Coordinates.Longitude));
			return sites;
		}

		public static IEnumerable<PlotExport> GetSitesExport(string country)
		{
			IEnumerable<Site> sites = GetSites(country);
			List<PlotExport> returnPlots = new List<PlotExport>();

			foreach (Site s in sites)
			{
				foreach (Plot sitePlot in s.Plots)
				{
					int plotId = sitePlot.Id;

					Site site = HomeController.sites.Value.SingleOrDefault(x => x.Plots.Any(plot => plot.Id == plotId)) ??
						HomeController.sitesConfidential.Value.SingleOrDefault(x => x.Plots.Any(plot => plot.Id == plotId));

					if (site != null)
					{
						Site result = site.Clone();
						result.Plots.RemoveAll(x => x.Id != plotId);
						returnPlots.Add(new PlotExport().Map(result));
					}
				}
			}

			return returnPlots;
		}

		public static IEnumerable<PlotExport> GetSitesExport(double longitudeWest, double lattitudeSouth, double longitudeEast, double lattitudeNorth)
		{
			IEnumerable<Site> sites = GetSites(longitudeWest, lattitudeSouth, longitudeEast, lattitudeNorth);
			List<PlotExport> returnPlots = new List<PlotExport>();

			foreach (Site s in sites)
			{
				foreach (Plot sitePlot in s.Plots)
				{
					int plotId = sitePlot.Id;

					Site site = HomeController.sites.Value.SingleOrDefault(x => x.Plots.Any(plot => plot.Id == plotId)) ??
						HomeController.sitesConfidential.Value.SingleOrDefault(x => x.Plots.Any(plot => plot.Id == plotId));

					if (site != null)
					{
						Site result = site.Clone();
						result.Plots.RemoveAll(x => x.Id != plotId);
						returnPlots.Add(new PlotExport().Map(result));
					}
				}
			}

			return returnPlots;
		}

		[HttpGet]
		public virtual JsonResult GetEmailAddress(string recipient)
		{
			switch (recipient)
			{
				case "dmitry":
					return Json("schepd@iiasa.ac.at");
				case "jerome":
					return Json("jerome.chave@univ-tlse3.fr");
			}

			return Json(null);
		}

		// GET: /<controller>/
		public IActionResult Index()
		{
			this.log.LogInformation("Hello, world!");

			IndexViewModel viewModel = new IndexViewModel
			{
				Countries = GetCountries(),
				PlotLocations = GetPlotLocations(),
				ConfidentialPlotLocations = GetConfidentialPlotLocations(),
				DownloadType = "geojson",
				AcceptTermsConditions = false,
				IntendedUse = string.Empty,
			};

			return View(viewModel);
		}

		[HttpGet]
		[Route("Plot/{plotId}")]
		public virtual IActionResult Plot(int plotId)
		{
			Site site = HomeController.sites.Value.SingleOrDefault(x => x.Plots.Any(plot => plot.Id == plotId)) ??
				HomeController.sitesConfidential.Value.SingleOrDefault(x => x.Plots.Any(plot => plot.Id == plotId));

			if (site != null)
			{
				Site result = site.Clone();
				result.Plots.RemoveAll(x => x.Id != plotId);
				return Json(new PlotViewModel().Map(result));
			}

			return Json(null);
		}

		private static ICollection<Site> GetConfidentialSites()
		{
			using (IFBNContext context = new IFBNContext())
			{
				return context.Features.Where(x => x.PlotType.Name == "Site" && x.Confidential)
					.Select(site => new Site()
					{
						Id = site.Id,
						Geometry = site.GeometryInternal,
						Name = site.Name,
						Url = site.Url,
						BiomassProcessingProtocol = site.BiomassProcessingProtocol,
						Country = site.Country,
						Established = site.Established,
						Reference = site.Reference,
						Network = site.Network,
						PIs = site.FeaturePrincipalInvestigators.Select(x => x.PrincipalInvestigator.Name).ToList(),
						PIs_text = site.PI_team,
						Institutions = site.FeatureInstitutions.Select(x => x.Institution.Name).ToList(),
						Photos = site.Photos.Select(x => x.FileName).ToList(),
						Plots = site.Children.Select(plot => new Plot()
						{
							Id = plot.Id,
							Geometry = plot.GeometryInternal,
							Geometry_Corner = plot.Geometry_CornerInternal,
							Name = plot.Name,
							Altitude = plot.Altitude,
							Area = plot.Area,
							Area_sum = site.Children.Where(x => x.ParentId == site.Id).Sum(sum => sum.Area),
							Slope = plot.Slope,
						})
							.ToList(),
					})
					.ToList();
			}
		}

		private static ICollection<Site> GetPublicSites()
		{
			using (IFBNContext context = new IFBNContext())
			{
				return context.Features.Where(x => x.PlotType.Name == "Site" && !x.Confidential)
					.Select(site => new Site()
					{
						Id = site.Id,
						Geometry = site.GeometryInternal,
						Name = site.Name,
						Url = site.Url,
						BiomassProcessingProtocol = site.BiomassProcessingProtocol,
						Country = site.Country,
						Established = site.Established,
						Reference = site.Reference,
						Network = site.Network,
						PIs = site.FeaturePrincipalInvestigators.Select(x => x.PrincipalInvestigator.Name).ToList(),
						PIs_text = site.PI_team,
						Institutions = site.FeatureInstitutions.Select(x => x.Institution.Name).ToList(),
						Photos = site.Photos.Select(x => x.FileName).ToList(),
						Plots = site.Children.Select(plot => new Plot()
						{
							Id = plot.Id,
							Geometry = plot.GeometryInternal,
							Geometry_Corner = plot.Geometry_CornerInternal,
							Name = plot.Name,
							Altitude = plot.Altitude,
							Area = plot.Area,
							Area_sum = site.Children.Where(x => x.ParentId == site.Id).Sum(sum => sum.Area),
							Slope = plot.Slope,
							Censuses = plot.Censuses.Select(census => new Models.Census()
							{
								Id = census.Id,
								From = census.From,
								To = census.To,
								Measurements =
										census.Measurements.Select(measurement => new Models.Measurement()
										{
											DateTime = measurement.DateTime,
											Type = measurement.TypeMethod.Type.Name.ToString(),
											Method = measurement.TypeMethod.Method.Name.ToString(),
											ValueInt = measurement.ValueInt,
											ValueFloat = measurement.ValueFloat,
											ValueString = measurement.ValueString,
											Unit = measurement.TypeUnit.Unit.Name.ToString(),
											UnitSymbol = measurement.TypeUnit.Unit.Symbol,
											ValueCode = measurement.TypeCode != null ? measurement.TypeCode.Code.Name.ToString()
													: null,
										})
											.ToList(),
								TaxonomicIdentifications = census.TaxonomicIdentifications
										.OrderByDescending(identification => identification.Percentage)
										.Select(identification => new Models.TaxonomicIdentification()
										{
											Count = identification.Count,
											Percentage = identification.Percentage,
											Family = identification.Species.Genus.Family.Name,
											Genus = identification.Species.Genus.Name,
											Species = identification.Species.Name,
											Year=identification.Year,
										})
										.Take(10)
										.ToList(),
							})
									.ToList(),
						})
							.ToList(),
					})
					.ToList();
			}
		}

		private List<PlotLocationViewModel> GetConfidentialPlotLocations()
		{
			ICollection<Site> sites = HomeController.sitesConfidential.Value;

			return new List<PlotLocationViewModel>().Map(sites);
		}

		private ICollection<string> GetCountries()
		{
			return HomeController.sites.Value.Select(x => x.Country).OrderBy(x => x).Distinct().ToList();
		}

		private List<PlotLocationViewModel> GetPlotLocations()
		{
			ICollection<Site> sites = HomeController.sites.Value;

			return new List<PlotLocationViewModel>().Map(sites);
		}
	}
}