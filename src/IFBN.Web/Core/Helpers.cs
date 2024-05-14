// <copyright file="Helpers.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Web.Core
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text;
	using IFBN.Web.Models;

	public static class Helpers
	{
		public static Site Clone(this Site site)
		{
			using (Stream stream = new MemoryStream())
			{
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, site);
				stream.Seek(0, SeekOrigin.Begin);

				return (Site)formatter.Deserialize(stream);
			}
		}

		public static byte[] GetBytes(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		public static string GetNiceDate(DateTime? dateTime)
		{
			if (!dateTime.HasValue)
			{
				return null;
			}

			if (dateTime.Value.Day == 1 && dateTime.Value.Month == 1)
			{
				return dateTime.Value.Year.ToString();
			}

			return dateTime.Value.ToString("dd MMMM, yyyy", CultureInfo.InvariantCulture);
		}

		public static string GetNiceDate(DateTime? from, DateTime? to)
		{
			if (!from.HasValue)
			{
				return null;
			}

			if (!to.HasValue)
			{
				return GetNiceDate(from);
			}

			if (from.Value.Year == to.Value.Year && from.Value.Day == 1 && from.Value.Month == 1 && to.Value.Day == 31 &&
				to.Value.Month == 12)
			{
				return from.Value.Year.ToString(CultureInfo.InvariantCulture);
			}

			return GetNiceDate(from) + " - " + GetNiceDate(to);
		}

		public static string GetValue(this Measurement measurement)
		{
			if (measurement.ValueInt.HasValue)
			{
				return measurement.ValueInt.Value.ToString();
			}

			if (measurement.ValueFloat.HasValue)
			{
				if (measurement.ValueFloat < 1)
				{
					return measurement.ValueFloat.Value.ToString("n3");
				}

				if (measurement.ValueFloat < 10)
				{
					return measurement.ValueFloat.Value.ToString("n2");
				}

				if (measurement.ValueFloat < 100)
				{
					return measurement.ValueFloat.Value.ToString("n1");
				}

				return ((int)measurement.ValueFloat.Value).ToString();
			}

			return measurement.ValueString;
		}

		public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				T element = collection.ElementAt(i);
				if (predicate(element))
				{
					collection.Remove(element);
					i--;
				}
			}
		}

		public static bool WithinRectangle(double lattitudeNorth,
							  double longitudeWest,
							  double lattitudeSouth,
							  double longitudeEast,
							  double lattitude,
							  double longitude)
		{
			if (lattitude > lattitudeNorth)
			{
				return false;
			}
			else if (lattitude < lattitudeSouth)
			{
				return false;
			}
			if (longitudeEast >= longitudeWest)
			{
				return (longitude >= longitudeWest) && (longitude <= longitudeEast);
			}
			else
			{
				return longitude >= longitudeWest;
			}
		}

		public static IEnumerable<PlotExport> Plotexport(IEnumerable<Site> sites)
		{
			List<PlotExport> plotResult = new List<PlotExport>();
			foreach (Site site in sites)
			{
				Site result = site.Clone();
				plotResult.Add(new PlotExport().Map(result));
			}

			return plotResult;
		}

		public static string ToCSV(this System.Data.DataTable table, string delimator)
		{
			var result = new StringBuilder();
			for (int i = 0; i < table.Columns.Count; i++)
			{
				result.Append(table.Columns[i].ColumnName);
				result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
			}
			foreach (System.Data.DataRow row in table.Rows)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					result.Append(row[i].ToString());
					result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
				}
			}
			return result.ToString().TrimEnd(new char[] { '\r', '\n' });
			//return result.ToString();
		}
	}
}