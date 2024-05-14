using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace IFBN.Web.Core
{
	public class JwkList
	{
		public List<JsonWebKey> Keys { get; set; }
	}
}