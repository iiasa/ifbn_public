﻿// <copyright file="Program.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web
{
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;

	public class Program
	{
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}
	}
}