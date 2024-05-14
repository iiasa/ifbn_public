// <copyright file="Census.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System;
	using System.Collections.Generic;

	public class Census
	{
		public virtual Feature Feature { get; set; }

		public DateTime? From { get; set; }

		public int Id { get; set; }

		public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

		public virtual ICollection<TaxonomicIdentification> TaxonomicIdentifications { get; set; } =
			new List<TaxonomicIdentification>();

		public DateTime? To { get; set; }
	}
}