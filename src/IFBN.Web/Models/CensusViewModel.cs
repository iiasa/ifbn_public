// <copyright file="CensusViewModel.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System.Collections.Generic;

	public class CensusViewModel
	{
		public string CensusDate { get; set; }

		public ICollection<MeasurementViewModel> Measurements { get; set; }

		public ICollection<string> TaxonomicIdentifications { get; set; }
	}
}