using System;
using System.Collections.Generic;
using System.Text;

namespace IFBN.Data.Entities
{
	public class Download
	{
		public int Id { get; set; }

		//public virtual ApplicationUser User { get; set; }

		public string UserId { get; set; }

		public string Filename { get; set; }

		public string IntendedUse { get; set; }

		public DateTime Timestamp { get; set; }

		public string Application { get; set; }
	}
}
