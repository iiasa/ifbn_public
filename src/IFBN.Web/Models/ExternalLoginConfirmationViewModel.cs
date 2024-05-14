// <copyright file="ExternalLoginConfirmationViewModel.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System.ComponentModel.DataAnnotations;
	using IFBN.Web.Core;
	using Microsoft.AspNetCore.Mvc;

	public class ExternalLoginConfirmationViewModel
	{
		// ReadOnly property does not get submitted in postback, but is not possible to edit anyways
		////[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Field { get; set; }

		[Required]
		[CheckBoolTrue(ErrorMessage = "Please accept the terms.")]
		[Display(Name = "Accept the terms below")]
		public bool HasAcceptedTerms { get; set; }

		[Required]
		public string Institution { get; set; }

		[Required]
		[StringLength(15, MinimumLength = 5, ErrorMessage = "Use 5 - 25 characters.")]
		[RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed.")]
		[Remote("IsUID_Available", "Account")]
		public string Username { get; set; }

		[Required]
		public string Affiliation { get; set; }
	}
}