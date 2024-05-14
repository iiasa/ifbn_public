// <copyright file="TypeUnit.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;

	public class TypeUnit
	{
		public double? Max { get; set; }

		public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

		public double? Min { get; set; }

		public virtual Type Type { get; set; }

		public int TypeId { get; set; }

		public virtual Unit Unit { get; set; }

		public int UnitId { get; set; }
	}
}