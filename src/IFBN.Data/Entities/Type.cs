// <copyright file="Type.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Type
	{
		public string Details { get; set; }

		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public virtual ICollection<TypeCode> TypeCodes { get; set; } = new List<TypeCode>();

		public virtual ICollection<TypeMethod> TypeMethods { get; set; } = new List<TypeMethod>();

		public virtual ICollection<TypeUnit> TypeUnits { get; set; } = new List<TypeUnit>();
	}
}