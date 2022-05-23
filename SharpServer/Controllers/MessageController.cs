using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SharpServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MessageController : ControllerBase
	{
		private readonly DatabaseContext databaseContext;

		public MessageController(DatabaseContext newDatabaseContext)
        {
			databaseContext = newDatabaseContext;
        }		

		[HttpGet]
		public IActionResult GetMessagesHistory()
		{
			if (databaseContext.Messages.Count() == 0)
            {
				return NotFound();
            }

			return Ok(databaseContext.Messages);
		}

		[HttpPost]
		public IActionResult AddMessage(string sender, string content)
		{
			Models.Message messageData = new Models.Message()
			{
				Id = databaseContext.Messages.Count() + 1,
				Content = content,
				Sender = sender,
			};

			databaseContext.Messages.Add(messageData);
			databaseContext.SaveChanges();
			return Ok();
		}
	}
}
