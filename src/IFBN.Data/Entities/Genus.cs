// <copyright file="Genus.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Genus
	{
		public string Details { get; set; }

		public virtual Family Family { get; set; }

		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual ICollection<Species> Species { get; set; } = new List<Species>();

		public virtual ICollection<TaxonomicIdentification> TaxonomicIdentifications { get; set; } =
	new List<TaxonomicIdentification>();
	}
}