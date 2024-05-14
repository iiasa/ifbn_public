// <copyright file="Feature.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using GeoAPI.Geometries;
	using NetTopologySuite.IO;

	public class Feature
	{
		public int? Altitude { get; set; }

		public double? Area { get; set; }

		public virtual ICollection<Census> Censuses { get; set; } = new List<Census>();

		public virtual ICollection<Feature> Children { get; set; } = new List<Feature>();

		public bool Confidential { get; set; }

		public string Country { get; set; }

		public virtual DateTime? Established { get; set; }

		public virtual ICollection<FeatureInstitution> FeatureInstitutions { get; set; } = new List<FeatureInstitution>();

		public virtual ICollection<FeaturePrincipalInvestigator> FeaturePrincipalInvestigators { get; set; } =
			new List<FeaturePrincipalInvestigator>();

		[NotMapped]
		public IGeometry Geometry
		{
			get => string.IsNullOrEmpty(GeometryInternal) ? null : new WKTReader().Read(GeometryInternal);
			set => GeometryInternal = value == null ? null : new WKTWriter().Write(value);
		}

		[Column("Geometry")]
		public string GeometryInternal { get; set; }

		[NotMapped]
		public IGeometry Geometry_Corner
		{
			get => string.IsNullOrEmpty(Geometry_CornerInternal) ? null : new WKTReader().Read(Geometry_CornerInternal);
			set => Geometry_CornerInternal = value == null ? null : new WKTWriter().Write(value);
		}

		[Column("Geometry_Corner")]
		public string Geometry_CornerInternal { get; set; }

		public int Id { get; set; }

		// Provide coarse location and round to <n> decimal places. if it is .5, then round to 0 or 5 after the n-th place
		public double? LocationRoundFactor { get; set; }

		[Required]
		public string Name { get; set; }

		public string Network { get; set; }

		public virtual Feature Parent { get; set; }

		public int? ParentId { get; set; }

		public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();

		public virtual PlotType PlotType { get; set; }

		public string Reference { get; set; }

		public int? Slope { get; set; }

		public string Url { get; set; }

		public string BiomassProcessingProtocol { get; set; }

		[Column("pi_team")]
		public string PI_team { get; set; }
	}
}