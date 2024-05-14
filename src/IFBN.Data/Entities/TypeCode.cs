// <copyright file="TypeCode.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class TypeCode
	{
		[Required]
		public virtual Code Code { get; set; }

		public int CodeId { get; set; }

		public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

		[Required]
		public virtual Type Type { get; set; }

		public int TypeId { get; set; }
	}
}