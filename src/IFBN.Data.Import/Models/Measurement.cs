// <copyright file="Measurement.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Import.Models
{
	using System;

	public class Measurement
	{
		public DateTime? DateTime { get; set; }

		public string Method { get; set; }

		public string Type { get; set; }

		public string Unit { get; set; }

		public string UnitSymbol { get; set; }

		// TODO: Merge to Value property?
		public string ValueCode { get; set; }

		public double? ValueFloat { get; set; }

		public int? ValueInt { get; set; }

		public string ValueString { get; set; }
	}
}