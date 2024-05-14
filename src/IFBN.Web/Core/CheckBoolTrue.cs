// <copyright file="CheckBoolTrue.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Core
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public class CheckBoolTrue : ValidationAttribute
	{
		public CheckBoolTrue()
			: base("The box must be checked.")
		{
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value != null)
			{
				try
				{
					if ((bool)value != true)
					{
						string errorMessage = FormatErrorMessage(validationContext.DisplayName);
						return new ValidationResult(errorMessage);
					}
				}
				catch (Exception)
				{
					return new ValidationResult("Input is not valid");
				}
			}

			return ValidationResult.Success;
		}
	}
}