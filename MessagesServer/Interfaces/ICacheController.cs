using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesServer.Interfaces
{
    public interface ICacheController : IDisposable
    {
        public bool OpenConnection();
        public bool CloseConnection();
        public string GetValueByKey(string key);
        public bool SetValue(string key, string value);
    }
}
