// <copyright file="Plot.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class Plot
	{
		public int? Altitude { get; set; }

		public double? Area { get; set; }

		public double? Area_sum { get; set; }

		public ICollection<Census> Censuses { get; set; }

		public string Geometry { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public int? Slope { get; set; }

		public string Geometry_Corner { get; set; }
	}
}