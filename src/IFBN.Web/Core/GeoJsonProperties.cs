using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFBN.Web.Models;

namespace IFBN.Web.Core
{
	public class GeoJsonProperties
	{
		public GeoJsonProperties(int id, string name, string country, Site data)
		{
			Id = id;
			Name = name;
			Country = country;
			Data = data.Clone();
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public string Country { get; set; }

		public Site Data { get; set; }
	}
}
