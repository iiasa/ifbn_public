// <copyright file="Code.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Code
	{
		public string Details { get; set; }

		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual ICollection<TypeCode> TypeCodes { get; set; } = new List<TypeCode>();
	}
}