// <copyright file="ApplicationUser.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using Microsoft.AspNetCore.Identity;

	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		public string Field { get; set; }

		public bool HasAcceptedTerms { get; set; }

		public string Institution { get; set; }

		public string Affiliation { get; set; }
	}
}