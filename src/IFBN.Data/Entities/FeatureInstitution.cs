// <copyright file="FeatureInstitution.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	public class FeatureInstitution
	{
		public virtual Feature Feature { get; set; }

		public int FeatureId { get; set; }

		public virtual Institution Institution { get; set; }

		public int InstitutionId { get; set; }
	}
}