// <copyright file="Species.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Species
	{
		public string Details { get; set; }

		public virtual Genus Genus { get; set; }

		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual ICollection<TaxonomicIdentification> TaxonomicIdentifications { get; set; } =
			new List<TaxonomicIdentification>();
	}
}