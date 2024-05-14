// <copyright file="Mappers.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using IFBN.Data.Contants;
	using IFBN.Web.Models;

	public static class Mappers
	{
		public static PlotExport Map(this PlotExport plotExportModel, Site site)
		{
			plotExportModel.SiteName = site.Name;
			plotExportModel.Country = site.Country;
			plotExportModel.Url = site.Url;
			plotExportModel.PIs = string.Join(", ", site.PIs);
			plotExportModel.PIs_text = site.PIs_text;
			plotExportModel.Institutions = string.Join(", ", site.Institutions);
			plotExportModel.Network = site.Network;
			plotExportModel.EstablishedDate = Helpers.GetNiceDate(site.Established);
			plotExportModel.Reference = site.Reference.Replace(';',',');
			plotExportModel.BiomassProcessingProtocol = site.BiomassProcessingProtocol;
			plotExportModel.Geometry = site.Geometry;

			Plot plot = site.Plots.First();
			plotExportModel.PlotName = plot.Name;

			if (plot.Altitude.HasValue && plot.Altitude.Value != 0)
			{
				plotExportModel.Altitude = $"{plot.Altitude.Value} m";
			}

			if (plot.Area.HasValue && plot.Area.Value != 0)
			{
				plotExportModel.Area = $"{plot.Area.Value} ha";
			}

			if (plot.Area_sum.HasValue && plot.Area_sum.Value != 0)
			{
				plotExportModel.Area_sum = $"{plot.Area_sum.Value} ha";
			}

			if (plot.Slope.HasValue && plot.Slope.Value != 0)
			{
				plotExportModel.Slope = $"{plot.Slope.Value} %";
			}

			string censusDate = "";
			string measurements ="";
			string additionalMeasurements = "";
			string taxonomicIdentifications = "";
			foreach (Census census in plot.Censuses ?? new List<Census>())
			{
				CensusViewModel censusViewModel=new CensusViewModel().Map(census);

				censusDate = censusViewModel.CensusDate;
				foreach (MeasurementViewModel measurementViewModel in censusViewModel.Measurements)
				{
					measurements = measurements + ", " + measurementViewModel.Measurement;

					foreach (string measurement in measurementViewModel.AdditionalMeasurements)
					{
						additionalMeasurements = additionalMeasurements + measurement;
					}
					
				}

				foreach (string taxonomicIdentification in censusViewModel.TaxonomicIdentifications)
				{
					taxonomicIdentifications = taxonomicIdentifications + taxonomicIdentification;
				}
			}

			plotExportModel.CensusDate = censusDate;
			plotExportModel.Measurements = measurements;
			plotExportModel.AdditionalMeasurements = additionalMeasurements;
			plotExportModel.TaxonomicIdentifications = taxonomicIdentifications;

			return plotExportModel;

		}

		public static List<PlotLocationViewModel> Map(this List<PlotLocationViewModel> viewModel, ICollection<Site> sites)
		{
			foreach (Site site in sites)
			{
				if (site.Plots != null)
				{
					foreach (Plot plot in site.Plots)
					{
						viewModel.Add(new PlotLocationViewModel
						{
							Id = plot.Id, PlotName = $"{site.Name} ({plot.Name})", GeometryWKT = plot.Geometry, Geometry_CornerWKT = plot.Geometry_Corner, Network = site.Network,
						});
					}
				}
			}

			return viewModel;
		}

		public static PlotViewModel Map(this PlotViewModel viewModel, Site site)
		{
			viewModel.SiteName = site.Name;
			viewModel.Country = site.Country;
			viewModel.Url = site.Url;
			viewModel.PIs = string.Join(", ", site.PIs);
			viewModel.PIs_text = site.PIs_text;
			viewModel.Institutions = string.Join(", ", site.Institutions);
			viewModel.Network = site.Network;
			viewModel.EstablishedDate = Helpers.GetNiceDate(site.Established);
			viewModel.Photos = site.Photos;
			viewModel.Reference = site.Reference;
			viewModel.BiomassProcessingProtocol = site.BiomassProcessingProtocol;

			Plot plot = site.Plots.First();
			viewModel.PlotName = plot.Name;

			if (plot.Altitude.HasValue && plot.Altitude.Value != 0)
			{
				viewModel.Altitude = $"{plot.Altitude.Value} m";
			}

			if (plot.Area.HasValue && plot.Area.Value != 0)
			{
				viewModel.Area = $"{plot.Area.Value} ha";
			}

			if (plot.Area_sum.HasValue && plot.Area_sum.Value != 0)
			{
				viewModel.Area_sum = $"{Math.Round(plot.Area_sum.Value, 2)} ha";
			}

			if (plot.Slope.HasValue && plot.Slope.Value != 0)
			{
				viewModel.Slope = $"{plot.Slope.Value} %";
			}

			viewModel.Censuses = new List<CensusViewModel>();
			foreach (Census census in plot.Censuses ?? new List<Census>())
			{
				viewModel.Censuses.Add(new CensusViewModel().Map(census));
			}

			return viewModel;
		}

		private static CensusViewModel Map(this CensusViewModel viewModel, Census census)
		{
			viewModel.CensusDate = Helpers.GetNiceDate(census.From, census.To);

			viewModel.Measurements = new List<MeasurementViewModel>().Map(census.Measurements);

			viewModel.TaxonomicIdentifications = new List<string>().Map(census.TaxonomicIdentifications);

			return viewModel;
		}

		private static List<MeasurementViewModel> Map(this List<MeasurementViewModel> viewModel, ICollection<Measurement> measurements)
		{
			foreach (Measurement measurement in measurements
				.Where(x => x.Method != Methods.LowerQuantil && x.Method != Methods.UpperQuantil)
				.OrderBy(x => x.Type))
			{
				MeasurementViewModel measurementViewModel = new MeasurementViewModel();

				if (measurement.Method == Methods.Average)
				{
					Measurement lower = measurements.FirstOrDefault(x => x.Type == measurement.Type && x.Method == Methods.LowerQuantil);
					Measurement upper = measurements.FirstOrDefault(x => x.Type == measurement.Type && x.Method == Methods.UpperQuantil);

					if (lower != null)
					{
						measurementViewModel.AdditionalMeasurements.Add($"{lower.Method}: {lower.GetValue()} {measurement.UnitSymbol}");
					}

					if (upper != null)
					{
						measurementViewModel.AdditionalMeasurements.Add($"{upper.Method}: {upper.GetValue()} {measurement.UnitSymbol}");
					}
				}

				string method = measurement.Method != Methods.Unknown && measurement.Method != Methods.Average ? measurement.Method
					: string.Empty;

				measurementViewModel.Measurement = $"{measurement.Type} {method}: {measurement.GetValue()} {measurement.UnitSymbol}";

				viewModel.Add(measurementViewModel);
			}

			return viewModel;
		}

		private static List<string> Map(this List<string> viewModel, ICollection<TaxonomicIdentification> taxonomicIdentifications)
		{
			foreach (TaxonomicIdentification taxonomic in taxonomicIdentifications)
			{
				////string tax = $"{taxonomic.Family} {taxonomic.Genus} {taxonomic.Species}: ";
				string tax = $"{taxonomic.Genus} {taxonomic.Species}: ";
				if (taxonomic.Percentage > 0)
				{
					tax += $"{taxonomic.Percentage} % ";
				}

				if (taxonomic.Count > 0)
				{
					tax += $" ({taxonomic.Count})";
				}

				//if (taxonomic.Year > 0)
				//{
				//	tax += $" ({taxonomic.Year})";
				//}

				viewModel.Add(tax);
			}

			return viewModel;
		}
	}
}