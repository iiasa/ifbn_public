// <copyright file="Program.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Import
{
	using System;
	using System.Globalization;
	using System.IO;
	using IFBN.Data.Contants;
	using IFBN.Data.Entities;
	using NetTopologySuite.Geometries;
	using SpreadsheetLight;

	public static class Program
	{
		public static void Main(string[] args)
		{
			// Import Sites, Plots and Taxonomies
			ImportExcels();
		}

		private static void ImportExcels()
		{
			IFBNContext context = new IFBNContext();

			// Don't do that in production
			//context.Database.EnsureDeleted();
			//context.Database.EnsureCreated();

			ImportHelper importHelper = new ImportHelper() { Context = context };

			importHelper.AddMissingTypes();
			importHelper.AddMissingUnits();
			importHelper.AddMissingMethods();

			importHelper.AddMissingTypeUnits();
			importHelper.AddMissingTypeMethods();

			importHelper.AddMissingCodes();

			importHelper.AddMissingPlotTypes();

			//ImportSites($"Data{Path.DirectorySeparatorChar}Plots_DB.xlsx", importHelper);
			ImportSites($"Data{Path.DirectorySeparatorChar}Plots_DB_upd_08_2022.xlsx", importHelper);
			context.SaveChanges();
			Console.WriteLine("Plots imported.");

			//ImportPlots($"Data{Path.DirectorySeparatorChar}SubPlots_DB.xlsx", importHelper);
			ImportPlots($"Data{Path.DirectorySeparatorChar}SubPlots_DB_upd_08_2022.xlsx", importHelper);
			context.SaveChanges();
			Console.WriteLine("Subplots imported.");

			//ImportTaxonomies($"Data{Path.DirectorySeparatorChar}Taxonomy.xlsx", importHelper);
			ImportTaxonomies($"Data{Path.DirectorySeparatorChar}Taxonomy_upd_08_2022.xlsx", importHelper);
			context.SaveChanges();
			Console.WriteLine("Taxonomies imported.");
		}

		private static void ImportPlots(string plotsFile, ImportHelper importHelper)
		{
			SLDocument document = new SLDocument(plotsFile);

			for (int i = 2; i <= document.GetWorksheetStatistics().NumberOfRows; i++)
			{
				string siteName = document.GetCellValueAsString(i, 1);
				string plotName = document.GetCellValueAsString(i, 2);

				double longitude = document.GetCellValueAsDouble(i, 4);
				double latitude = document.GetCellValueAsDouble(i, 3);

				////Feature site = importHelper.GetAndAddIfMissingSite(siteName);
				Feature site = importHelper.GetSite(siteName);
				if (site == null)
				{
					continue;
				}

				if (site.LocationRoundFactor.HasValue)
				{
					latitude = latitude.Obfuscate(site.LocationRoundFactor.Value);
					longitude = longitude.Obfuscate(site.LocationRoundFactor.Value);
				}

				Point point = new Point(longitude, latitude);

				site.Geometry = site.Geometry == null ? point.Envelope : new GeometryCollection(new[] { site.Geometry, point }).Envelope;

				Feature plot = importHelper.GetAndAddIfMissingPlot(site, plotName);
				plot.Geometry = point;

				//check if it's a circle
				string plot_shape = document.GetCellValueAsString(i, 16);
				bool iscircle = false;
				double radius = -1;
				double radius_m = -1;
				if (plot_shape.ToLower().StartsWith("circle"))
				{
					string tmpradius = plot_shape.Trim().Substring(plot_shape.IndexOf("r=") + 2);
					tmpradius = tmpradius.Substring(0, tmpradius.Length - 1).Trim();
					//radius = double.Parse(tmpradius) ;
					//iscircle = true;

					iscircle=double.TryParse(tmpradius, NumberStyles.Any, CultureInfo.InvariantCulture, out radius);
					radius_m = radius;
					
				}

				
				
				if (iscircle == true)
				{
					//if it's circle

					//radius in degree
					//1km (1000m) ==  0.0083333333333333333333;
					//double km = 0.0083333333333333333333;
					//radius = km / 1000 * radius;



					var y = latitude;
					var x = longitude;

					//calculate length
					var lat = y * Math.PI / 180;

					// Set up "Constants"
					var m1 = 111132.92;     // latitude calculation term 1
					var m2 = -559.82;       // latitude calculation term 2
					var m3 = 1.175;         // latitude calculation term 3
					var m4 = -0.0023;       // latitude calculation term 4
					var p1 = 111412.84;     // longitude calculation term 1
					var p2 = -93.5;         // longitude calculation term 2
					var p3 = 0.118;         // longitude calculation term 3

					// Calculate the length of a degree of latitude and longitude in meters
					var latlen = m1 + (m2 * Math.Cos(2 * lat)) + (m3 * Math.Cos(4 * lat)) +
						(m4 * Math.Cos(6 * lat));
					var longlen = (p1 * Math.Cos(lat)) + (p2 * Math.Cos(3 * lat)) +
						(p3 * Math.Cos(5 * lat)); //in meter

					//in km
					latlen = latlen / 1000;
					longlen = longlen / 1000;

					var latkm = 1 / latlen;
					var lonkm = 1 / longlen;

					
					//var cellsize = 1 / (longlen/1000 );
					var cellsize_lat = latkm * radius_m/1000;
					var cellsize_lon = lonkm * radius_m / 1000;

					var lat_top = y - (cellsize_lat);
					var lat_bottom = y + (cellsize_lat);
					var lon_left = x - (cellsize_lon);
					var lon_right = x + (cellsize_lon);

					//Geometry_Corner
					var sw = new GeoAPI.Geometries.Coordinate(lon_left, lat_bottom);
					var nw = new GeoAPI.Geometries.Coordinate(lon_left, lat_top);
					var se = new GeoAPI.Geometries.Coordinate(lon_right, lat_bottom);
					var ne = new GeoAPI.Geometries.Coordinate(lon_right, lat_top);


					var coordinatesq = new[] { sw, nw,  ne, se, sw };

					var coord = new GeometryFactory().CreateMultiPointFromCoords(coordinatesq);
					var circle = new NetTopologySuite.Algorithm.MinimumBoundingCircle(coord);
					plot.Geometry_Corner = circle.GetCircle();
				}
				else
				{
					//Geometry_Corner
					
					if (document.GetCellValueAsDouble(i, 5) == 0 || document.GetCellValueAsDouble(i, 6) == 0 ||
						document.GetCellValueAsDouble(i, 7) == 0 || document.GetCellValueAsDouble(i, 8) == 0 ||
						document.GetCellValueAsDouble(i, 9) == 0 || document.GetCellValueAsDouble(i, 10) == 0 ||
						document.GetCellValueAsDouble(i, 11) == 0 || document.GetCellValueAsDouble(i, 12) == 0)
					{
						plot.Geometry_Corner = null;
					}
					else {
						var sw = new GeoAPI.Geometries.Coordinate(document.GetCellValueAsDouble(i, 6), document.GetCellValueAsDouble(i, 5));
						var nw = new GeoAPI.Geometries.Coordinate(document.GetCellValueAsDouble(i, 8), document.GetCellValueAsDouble(i, 7));
						var se = new GeoAPI.Geometries.Coordinate(document.GetCellValueAsDouble(i, 10), document.GetCellValueAsDouble(i, 9));
						var ne = new GeoAPI.Geometries.Coordinate(document.GetCellValueAsDouble(i, 12), document.GetCellValueAsDouble(i, 11));
						var coordinatesq = new[] { sw, nw, ne, se, sw };
						plot.Geometry_Corner = new GeometryFactory().CreatePolygon(coordinatesq);
					}
				}
				
				Census census;

				string date = document.GetCellValueAsString(i, 18);
				if (DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
				{
					census = importHelper.GetAndAddIfMissingCensus(plot, dateTime, dateTime);
				}
				else
				{
					if (!int.TryParse(date, out int year))
					{
						year = 2018;
					}

					census = importHelper.GetAndAddIfMissingCensus(plot, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
				}

				// Measurements
				plot.Altitude = document.GetCellValueAsInt32(i, 13);
				plot.Slope = document.GetCellValueAsInt32(i, 14);
				plot.Area = (double)document.GetCellValueAsDecimal(i, 15);

				if (site.Confidential)
				{
					continue;
				}

				// TODO: Not working
				////importHelper.AddMeasurementCode(census, Types.ForestStatus, Units.Unknown,
				////	document.GetCellValueAsString(i, 17));

				importHelper.AddMeasurement(census, Types.MinDBH, Units.Centimeter, document.GetCellValueAsInt32(i, 19));

				importHelper.AddMeasurement(census, Types.HLorey, Units.Meter, document.GetCellValueAsDouble(i, 20), Methods.Local);
				importHelper.AddMeasurement(census, Types.HLorey, Units.Meter, document.GetCellValueAsDouble(i, 21), Methods.Chave);
				importHelper.AddMeasurement(census, Types.HLorey, Units.Meter, document.GetCellValueAsDouble(i, 22), Methods.Feldpausch);

				importHelper.AddMeasurement(census, Types.HMax, Units.Meter, document.GetCellValueAsDouble(i, 23), Methods.Local);
				importHelper.AddMeasurement(census, Types.HMax, Units.Meter, document.GetCellValueAsDouble(i, 24), Methods.Chave);
				importHelper.AddMeasurement(census, Types.HMax, Units.Meter, document.GetCellValueAsDouble(i, 25), Methods.Feldpausch);

				importHelper.AddMeasurement(census, Types.AGBLocalHD, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 26),
					Methods.Average);
				importHelper.AddMeasurement(census, Types.AGBLocalHD, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 27),
					Methods.LowerQuantil);
				importHelper.AddMeasurement(census, Types.AGBLocalHD, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 28),
					Methods.UpperQuantil);

				importHelper.AddMeasurement(census, Types.AGBFeldpausch, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 29),
					Methods.Average);
				importHelper.AddMeasurement(census, Types.AGBFeldpausch, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 30),
					Methods.LowerQuantil);
				importHelper.AddMeasurement(census, Types.AGBFeldpausch, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 31),
					Methods.UpperQuantil);

				importHelper.AddMeasurement(census, Types.AGBChave, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 32),
					Methods.Average);
				importHelper.AddMeasurement(census, Types.AGBChave, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 33),
					Methods.LowerQuantil);
				importHelper.AddMeasurement(census, Types.AGBChave, Units.TonsPerHectare, document.GetCellValueAsDouble(i, 34),
					Methods.UpperQuantil);

				importHelper.AddMeasurement(census, Types.WoodDensity, Units.TonsPerCubicMeter, document.GetCellValueAsDouble(i, 35));
				importHelper.AddMeasurement(census, Types.GSV, Units.CubicMeterPerHectare, document.GetCellValueAsDouble(i, 36));
				importHelper.AddMeasurement(census, Types.BasalArea, Units.SquareMeterPerHectare, document.GetCellValueAsDouble(i, 37));
				importHelper.AddMeasurement(census, Types.Ndens, Units.TreesPerHectare, document.GetCellValueAsInt32(i, 38));

				if (i % 100 == 0)
				{
					importHelper.Context.SaveChanges();

					Console.WriteLine($"{i} subplots");
				}
			}
		}

		private static void ImportSites(string sitesFile, ImportHelper importHelper)
		{
			SLDocument document = new SLDocument(sitesFile);

			for (int i = 2; i <= document.GetWorksheetStatistics().NumberOfRows; i++)
			{
				string siteName = document.GetCellValueAsString(i, 1).Trim();
				if (string.IsNullOrEmpty(siteName))
				{
					continue;
				}

				Feature site = importHelper.GetAndAddIfMissingSite(siteName);

				site.Country = document.GetCellValueAsString(i, 2);

				importHelper.AddIfMissingInstitution(site, document.GetCellValueAsString(i, 3).Trim(), true);

				foreach (string institution in document.GetCellValueAsString(i, 4).SplitPIs())
				{
					if (!string.IsNullOrEmpty(institution.Trim()))
					{
						importHelper.AddIfMissingInstitution(site, institution.Trim());
					}
				}

				site.Url = document.GetCellValueAsString(i, 5);

				// TODO: Forest Status
				if (int.TryParse(document.GetCellValueAsString(i, 6), out int year))
				{
					site.Established = new DateTime(year, 1, 1);
				}

				// TODO: Institutions <-> Principial Investigators
				foreach (string pi in document.GetCellValueAsString(i, 7).Split(','))
				{
					importHelper.AddIfMissingPrincipalInvestigator(site, pi.Trim());
				}

				//read PI_team
				site.PI_team= document.GetCellValueAsString(i, 7);

				site.Reference = document.GetCellValueAsString(i, 8);

				// Confidential?
				if (document.GetCellValueAsString(i, 9).Trim().ToLowerInvariant() != "no")
				{
					site.Confidential = true;
				}

				// Other measurements?!

				// Provide only coarse location?
				if (double.TryParse(document.GetCellValueAsString(i, 11), out double round))
				{
					site.LocationRoundFactor = round;
				}

				if (File.Exists($"Images/{siteName}.jpg"))
				{
					site.Photos.Add(new Photo() { FileName = $"{site.Name}.jpg" });
				}

				// biomass processing protocol
				site.BiomassProcessingProtocol = document.GetCellValueAsString(i, 12);
				
			}
		}

		private static void ImportTaxonomies(string taxonomyFile, ImportHelper importHelper)
		{
			SLDocument document = new SLDocument(taxonomyFile);

			for (int i = 2; i <= document.GetWorksheetStatistics().NumberOfRows; i++)
			{
				string siteName = document.GetCellValueAsString(i, 1);
				string plotName = document.GetCellValueAsString(i, 2);

				Feature site = importHelper.GetAndAddIfMissingSite(siteName);

				if (site.Confidential)
				{
					continue;
				}

				Feature plot = importHelper.GetAndAddIfMissingPlot(site, plotName);
				Census census = importHelper.GetLastCensus(plot);

				if (census == null)
				{
					continue;
				}

				string family = document.GetCellValueAsString(i, 3);
				string genus = document.GetCellValueAsString(i, 4);
				string species = document.GetCellValueAsString(i, 5);
				int percentage = document.GetCellValueAsInt32(i, 6);
				int count = document.GetCellValueAsInt32(i, 7);
				int year = document.GetCellValueAsInt32(i, 8);

				Family familyEntity = importHelper.GetAndAddIfMissingFamily(family);
				Genus genusEntity = importHelper.GetAndAddIfMissingGenus(familyEntity, genus);
				Species speciesEntity = importHelper.GetAndAddIfMissingSpecies(genusEntity, species);

				census.TaxonomicIdentifications.Add(new TaxonomicIdentification()
				{
					Count = count, Percentage = percentage, Species = speciesEntity, Year = year,
				});

				if (i % 100 == 0)
				{
					importHelper.Context.SaveChanges();

					Console.WriteLine($"{i} taxonomies");
				}
			}
		}
	}
}