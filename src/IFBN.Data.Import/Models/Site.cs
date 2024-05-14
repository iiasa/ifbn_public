// <copyright file="Site.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Import.Models
{
	using System;
	using System.Collections.Generic;

	public class Site
	{
		public string Country { get; set; }

		public DateTime Established { get; set; }

		public string Geometry { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public ICollection<string> Photos { get; set; }

		public ICollection<Plot> Plots { get; set; }

		public string Url { get; set; }

		public string BiomassProcessingProtocol { get; set; }
	}
}