// <copyright file="Plot.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Import.Models
{
	using System.Collections.Generic;

	public class Plot
	{
		public ICollection<Census> Censuses { get; set; }

		public string Geometry { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public string Geometry_Corner { get; set; }
	}
}