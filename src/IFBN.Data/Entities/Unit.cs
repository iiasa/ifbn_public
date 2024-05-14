// <copyright file="Unit.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Unit
	{
		public string Details { get; set; }

		public int Id { get; set; }

		public double? Max { get; set; }

		public double? Min { get; set; }

		[Required]
		public string Name { get; set; }

		public string Symbol { get; set; }

		public virtual ICollection<TypeUnit> TypeUnits { get; set; } = new List<TypeUnit>();
	}
}