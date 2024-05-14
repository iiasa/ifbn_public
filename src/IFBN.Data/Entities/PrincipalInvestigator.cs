// <copyright file="PrincipalInvestigator.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class PrincipalInvestigator
	{
		public string Email { get; set; }

		public virtual ICollection<FeaturePrincipalInvestigator> FeaturePrincipalInvestigators { get; set; } =
			new List<FeaturePrincipalInvestigator>();

		public int Id { get; set; }

		public virtual Institution Institution { get; set; }

		[Required]
		public string Name { get; set; }
	}
}