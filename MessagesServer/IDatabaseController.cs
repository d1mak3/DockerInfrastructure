namespace MessagesServer
{
    public interface IDatabaseController : IDisposable
    {
        public bool OpenConnection();
        public bool CloseConnection();
        public bool SaveMessage(Models.Message messageToSave);
        public List<Models.Message> GetMessages();
    }
}
