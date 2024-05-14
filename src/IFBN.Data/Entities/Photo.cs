// <copyright file="Photo.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System;
	using System.ComponentModel.DataAnnotations;

	public class Photo
	{
		public DateTime? DateTime { get; set; }

		[Required]
		public virtual Feature Feature { get; set; }

		public string Geometry { get; set; }

		public int Id { get; set; }

		public short? Orientation { get; set; }

		public string FileName { get; set; }
	}
}