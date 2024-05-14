// <copyright file="IndexViewModel.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System.Collections.Generic;

	public class IndexViewModel
	{
		public ICollection<PlotLocationViewModel> ConfidentialPlotLocations { get; set; }

		public ICollection<string> Countries { get; set; }

		public ICollection<PlotLocationViewModel> PlotLocations { get; set; }

		public string DownloadType { get; set; } //Download geojson or csv

		public bool AcceptTermsConditions { get; set; } //before downloading: accept terms and conditions

		public string IntendedUse { get; set; } //mandatory before downloading
	}
}