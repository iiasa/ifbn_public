// <copyright file="Census.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class Census
	{
		public DateTime? From { get; set; }

		public int Id { get; set; }

		public ICollection<Measurement> Measurements { get; set; }

		public ICollection<TaxonomicIdentification> TaxonomicIdentifications { get; set; }

		public DateTime? To { get; set; }
	}
}