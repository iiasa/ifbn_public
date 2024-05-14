// <copyright file="PlotLocationViewModel.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	public class PlotLocationViewModel
	{
		public string GeometryWKT { get; set; }
		
		public string Geometry_CornerWKT { get; set; }

		public int Id { get; set; }

		public string PlotName { get; set; }

		public string Network { get; set; }

	}
}