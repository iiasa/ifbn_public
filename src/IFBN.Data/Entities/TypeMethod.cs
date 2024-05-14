// <copyright file="TypeMethod.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class TypeMethod
	{
		public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

		[Required]
		public virtual Method Method { get; set; }

		public int MethodId { get; set; }

		[Required]
		public virtual Type Type { get; set; }

		public int TypeId { get; set; }
	}
}