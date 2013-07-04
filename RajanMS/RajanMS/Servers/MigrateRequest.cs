using System;

namespace RajanMS.Servers
{
    class MigrateRequest
    {
        public DateTime Expiry { get; private set; }
        //public string Endpoint { get; private set; }
        public int CharacterId { get; private set; }
        public long SessionId { get; private set; }

        public MigrateRequest(/*string endpoint,*/int charId,long sessionId)
        {
            Expiry = DateTime.Now;
            //Endpoint = endpoint;
            CharacterId = charId;
            SessionId = sessionId;
        }
    }
}
