// <copyright file="TaxonomicIdentification.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.ComponentModel.DataAnnotations;

	// TODO: Check Species -> Genus -> Family
	public class TaxonomicIdentification
	{
		[Required]
		public virtual Census Census { get; set; }

		public int Count { get; set; }

		public long Id { get; set; }

		public int Percentage { get; set; }

		public int Year { get; set; }

		[Required]
		public virtual Species Species { get; set; }
	}
}