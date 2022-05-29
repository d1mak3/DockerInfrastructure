using MySql.Data.MySqlClient;
using System.Data.Common;

namespace MessagesServer.Controllers
{
	public class MySqlDatabaseController : IDatabaseController
	{
		private readonly MySqlConnection connection;

		public MySqlDatabaseController()
        {
			string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
				"Server=localhost;Port=3306;Database=messenger_database;Uid=root;Pwd=no_piko;";

			connection = new MySqlConnection(connectionString);

			if (!OpenConnection())
            {				
				throw new Exception("Couldn't connect to the database!");
            }
        }

		public bool OpenConnection()
        {
            try
            {
				connection.Open();
				return true;
            }
			catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

		public bool CloseConnection()
        {
			try
            {
				connection.Close();
				return true;
            }
			catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

		public bool SaveMessage(Models.Message messageToSave)
        {
			messageToSave.Id = GetMessages().Count + 1;

            try
            {
				MySqlCommand newCommand = connection.CreateCommand();				
				newCommand.CommandText = "INSERT INTO messages VALUES (" + messageToSave.ToString() + ");";
				newCommand.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
            {
				Console.WriteLine(ex.ToString());
				return false;
            }
        }

		public List<Models.Message> GetMessages()
        {
			List<Models.Message> messages = new List<Models.Message>();

			MySqlCommand newCommand = connection.CreateCommand();
			newCommand.CommandText = "SELECT * FROM messages;";
			newCommand.Connection = connection;
			
			using (DbDataReader reader = newCommand.ExecuteReader())
            {
				while (reader.Read())
                {
					messages.Add(new Models.Message
					{
						Id = reader.GetInt32(0),
						Sender = reader.GetString(1),
						Content = reader.GetString(2)
					});
                }
            }

			return messages;
		}

        public void Dispose()
        {
			CloseConnection();
        }
    }
}
