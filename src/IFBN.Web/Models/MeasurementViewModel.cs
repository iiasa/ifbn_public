// <copyright file="MeasurementViewModel.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Models
{
	using System.Collections.Generic;

	public class MeasurementViewModel
	{
		public MeasurementViewModel()
		{
			AdditionalMeasurements = new List<string>();
		}

		public ICollection<string> AdditionalMeasurements { get; set; }

		public string Measurement { get; set; }
	}
}