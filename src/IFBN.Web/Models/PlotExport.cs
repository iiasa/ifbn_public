using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFBN.Web.Models
{
	public class PlotExport
	{
		public string SiteName { get; set; }

		public string PlotName { get; set; }

		public string Country { get; set; }

		public string Network { get; set; }

		public string Institutions { get; set; }

		public string Url { get; set; }

		public string PIs { get; set; }

		public string PIs_text { get; set; }

		public string EstablishedDate { get; set; }

		public string Area { get; set; }

		public string Area_sum { get; set; }

		public string Altitude { get; set; }

		public string Slope { get; set; }

		public string BiomassProcessingProtocol { get; set; }

		public string CensusDate { get; set; }

		public string Measurements { get; set; }

		public string AdditionalMeasurements { get; set; }

		public string TaxonomicIdentifications { get; set; }

		public string Reference { get; set; }

		public string Geometry { get; set; }
	}
}