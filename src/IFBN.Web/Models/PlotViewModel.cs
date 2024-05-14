// <copyright file="PlotViewModel.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System.Collections.Generic;

	public class PlotViewModel
	{
		public string Altitude { get; set; }

		public string Area { get; set; }

		public string Area_sum { get; set; }

		public List<CensusViewModel> Censuses { get; set; }

		public string Country { get; set; }

		public string EstablishedDate { get; set; }

		public string Institutions { get; set; }

		public string Network { get; set; }

		public ICollection<string> Photos { get; set; }

		public string PIs { get; set; }

		public string PIs_text { get; set; }

		public string PlotName { get; set; }

		public string Reference { get; set; }

		public string SiteName { get; set; }

		public string Slope { get; set; }

		public string Url { get; set; }

		public string BiomassProcessingProtocol { get; set; }
	}
}