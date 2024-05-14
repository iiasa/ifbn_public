using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Wkx;

namespace IFBN.Web.Core
{
	using Newtonsoft.Json;
	using System.Data;
	using System.IO;
	using System.Xml;

	public static class GeoJsonHelper
	{
		public static GeoJSON.Net.Geometry.Point WktStringToPoint(string wktString)
		{
			Geometry geometry = Geometry.Deserialize<WktSerializer>(wktString);

			if (geometry.GeometryType == GeometryType.Point)
			{
				Wkx.Point p = geometry as Wkx.Point;

				return new GeoJSON.Net.Geometry.Point(new Position(p.Y.Value, p.X.Value));
			}
			else if (geometry.GeometryType == GeometryType.Polygon)
			{
				Wkx.Polygon poly = geometry as Wkx.Polygon;
				Wkx.Point center = poly.ExteriorRing.GetCenter();
				return new GeoJSON.Net.Geometry.Point(new Position(center.Y.Value, center.X.Value));
			}
			else if (geometry.GeometryType == GeometryType.LineString)
			{
				Wkx.LineString line = geometry as Wkx.LineString;
				Wkx.Point center = line.GetCenter();
				return new GeoJSON.Net.Geometry.Point(new Position(center.Y.Value, center.X.Value));
			}

			throw new ArgumentException("WKT String couldn't be converted to point.", nameof(wktString));
		}

		public static GeoJSON.Net.Geometry.Polygon WktStringToPoly(string wktString)
		{
			Geometry geometry = Geometry.Deserialize<WktSerializer>(wktString);

			if (geometry.GeometryType == GeometryType.Polygon)
			{
				Wkx.Polygon poly = geometry as Wkx.Polygon;
				List<Wkx.Point> points = poly.ExteriorRing.Points.ToList();
				List<GeoJSON.Net.Geometry.LineString> linestrings = new List<GeoJSON.Net.Geometry.LineString>();
				List<Position> positions = new List<Position>();
				points.ForEach(x => positions.Add(new Position(x.Y.Value, x.X.Value)));
				linestrings.Add(new GeoJSON.Net.Geometry.LineString(positions));

				GeoJSON.Net.Geometry.Polygon polygon = new GeoJSON.Net.Geometry.Polygon(linestrings);

				return polygon;
			}

			throw new ArgumentException("WKT String couldn't be converted to polygon.", nameof(wktString));
		}

		public static GeoJSON.Net.Geometry.IGeometryObject WktStringToGeometry(string wktString)
		{
			Geometry geometry = Geometry.Deserialize<WktSerializer>(wktString);

			if (geometry.GeometryType == GeometryType.Point)
			{
				// Wkx.Point p = geometry as Wkx.Point;

				// return new GeoJSON.Net.Geometry.Point(new Position(p.Y.Value, p.X.Value));

				return WktStringToPoint(wktString);
			}
			else if (geometry.GeometryType == GeometryType.Polygon)
			{
				// Wkx.Polygon poly = geometry as Wkx.Polygon;
				// List<Wkx.Point> points = poly.ExteriorRing.Points.ToList();
				// List<GeoJSON.Net.Geometry.LineString> linestrings = new List<GeoJSON.Net.Geometry.LineString>();
				// List<Position> positions = new List<Position>();
				// points.ForEach(x => positions.Add(new Position(x.Y.Value, x.X.Value)));
				// linestrings.Add(new GeoJSON.Net.Geometry.LineString(positions));

				// GeoJSON.Net.Geometry.Polygon polygon = new GeoJSON.Net.Geometry.Polygon(linestrings);

				// return polygon;

				return WktStringToPoly(wktString);
			}

			throw new ArgumentException("WKT String couldn't be converted to point.", nameof(wktString));
		}

		//public static void jsonStringToCSV(string jsonContent)
		//{
		//	//used NewtonSoft json nuget package
		//	XmlNode xml = JsonConvert.DeserializeXmlNode("{records:{record:" + jsonContent + "}}");
		//	XmlDocument xmldoc = new XmlDocument();
		//	xmldoc.LoadXml(xml.InnerXml);
		//	XmlReader xmlReader = new XmlNodeReader(xml);
		//	DataSet dataSet = new DataSet();
		//	dataSet.ReadXml(xmlReader);
		//	var dataTable = dataSet.Tables[1];

		//	//Datatable to CSV
		//	var lines = new List<string>();
		//	string[] columnNames = dataTable.Columns.Cast<DataColumn>().
		//		Select(column => column.ColumnName).
		//		ToArray();
		//	var header = string.Join(",", columnNames);
		//	lines.Add(header);
		//	//var valueLines = dataTable.AsEnumerable()
		//	//	.Select(row => string.Join(",", row.ItemArray));
		//	var valueLines = dataTable.Rows .AsQueryable(row => string.Join(",", row.ItemArray));
		//	lines.AddRange(valueLines);
		//	File.WriteAllLines(@"E:/Export.csv", lines);
		//}


	}
}