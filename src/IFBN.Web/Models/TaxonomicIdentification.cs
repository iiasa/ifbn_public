// <copyright file="TaxonomicIdentification.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System;

	[Serializable]
	public class TaxonomicIdentification
	{
		public int Count { get; set; }

		public string Family { get; set; }

		public string Genus { get; set; }

		public int Percentage { get; set; }

		public string Species { get; set; }

		public int Year { get; set; }
	}
}