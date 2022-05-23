using System;

namespace SharpServer.Models
{
	[Serializable]
	public class Message
	{
        public int Id { get; set; }
		public string Sender { get; set; }
		public string Content { get; set; }
	}
}
