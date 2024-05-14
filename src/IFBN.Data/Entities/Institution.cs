// <copyright file="Institution.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Institution
	{
		public string Address { get; set; }

		public virtual ICollection<FeatureInstitution> FeatureInstitutions { get; set; } = new List<FeatureInstitution>();

		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual ICollection<PrincipalInvestigator> PrincipalInvestigators { get; set; } =
			new List<PrincipalInvestigator>();
	}
}