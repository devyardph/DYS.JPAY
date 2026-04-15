using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DYS.JPay.Common.Dtos
{
	public class NotificationDto
	{
		public bool? Success { get; set; } = null;
		public string Title { get; set; } = "Notice";
		public string Description { get; set; } = string.Empty;
		public List<string> Messages { get; set; } = new List<string>();
	}
}
