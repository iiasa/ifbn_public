// <copyright file="JsonHelper.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Core
{
	using System.Diagnostics;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	public static class JsonHelper
	{
		private static Formatting Formatting
		{
			get
			{
				return Debugger.IsAttached ? Formatting.Indented : Formatting.None;
			}
		}

		public static string EncodeCamelCase(object value)
		{
			return JsonConvert.SerializeObject(value, Formatting,
				new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
		}
	}
}