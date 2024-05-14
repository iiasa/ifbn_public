// <copyright file="Measurement.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System;
	using System.ComponentModel.DataAnnotations;

	public class Measurement
	{
		public virtual Census Census { get; set; }

		public DateTime? DateTime { get; set; }

		public int Id { get; set; }

		public virtual Measurement Parent { get; set; }

		public virtual TypeCode TypeCode { get; set; }

		[Required]
		public virtual TypeMethod TypeMethod { get; set; }

		[Required]
		public virtual TypeUnit TypeUnit { get; set; }

		public double? ValueFloat { get; set; }

		public int? ValueInt { get; set; }

		public string ValueString { get; set; }
	}
}