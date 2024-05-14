// <copyright file="FeaturePrincipalInvestigator.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	public class FeaturePrincipalInvestigator
	{
		public virtual Feature Feature { get; set; }

		public int FeatureId { get; set; }

		public virtual PrincipalInvestigator PrincipalInvestigator { get; set; }

		public int PrincipalInvestigatorId { get; set; }
	}
}