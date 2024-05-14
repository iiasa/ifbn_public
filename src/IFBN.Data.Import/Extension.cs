// <copyright file="Extension.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data.Import
{
	using System;
	using System.Collections.Generic;

	public static class Extension
	{
		/// <summary>
		///     Obfuscate a double value (e.g. coordinate part) by rounding it to roundFactor number of decimal places.
		/// </summary>
		/// <param name="value">Number to be rounded.</param>
		/// <param name="roundFactor">Number of decimal places. Integer or .5 value.</param>
		/// <returns>Rounded number.</returns>
		public static double Obfuscate(this double value, double roundFactor)
		{
			int pow = (int)roundFactor;

			value = value * Math.Pow(10, pow);

			if (Math.Abs(roundFactor - (int)roundFactor) < 0.0001)
			{
				value = Math.Round(value);
			}
			else if (Math.Abs((roundFactor % 1) - 0.5) < 0.0001)
			{
				value = Math.Round(value * 2) / 2;
			}
			else
			{
				throw new ArgumentException("RoundFactor should be integer or .5 value.");
			}

			return value / Math.Pow(10, pow);
		}

		/// <summary>
		///     Split a string by comma but not when enclosed in parentheses.
		/// </summary>
		/// <param name="input">String to split.</param>
		/// <returns>Individual substrings.</returns>
		public static List<string> SplitPIs(this string input)
		{
			List<string> pis = new List<string>();
			bool skipComma = false;
			int start = 0;

			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];

				switch (c)
				{
					case '(':
						skipComma = true;
						break;
					case ')':
						skipComma = false;
						break;
					case ',' when !skipComma:
						pis.Add(input.Substring(start, i - start).Trim());
						start = ++i;
						break;
				}
			}

			pis.Add(input.Substring(start).Trim());

			return pis;
		}
	}
}