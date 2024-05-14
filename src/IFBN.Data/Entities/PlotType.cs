// <copyright file="PlotType.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.ComponentModel.DataAnnotations;

	public class PlotType
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
	}
}