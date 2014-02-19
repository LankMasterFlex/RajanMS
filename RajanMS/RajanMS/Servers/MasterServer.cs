using RajanMS.Tools;
using System;
using System.Collections.Generic;

namespace RajanMS.Servers
{
    public sealed class MasterServer
    {
        public static MasterServer Instance { get; set; }

        public bool Running { get; private set; }

        public LoginServer Login { get; private set; }
        public ChannelServer[] Channels { get; private set; }

        public Config Config { get; private set; }

        private List<string> m_loginPool;

        public MasterServer()
        {
            Config = new Config(Constants.ConfigName);

            Database.Instance = new Database(Config["Host"], Config["Name"]);

            byte channels = (byte)Config.GetInt("Channels");

            if (channels > 20)
                throw new Exception("More than 20 channels");

            Login = new LoginServer(8484);

            Channels = new ChannelServer[channels];

            short port = 8585;

            for (int i = 0; i < channels; i++)
            {
                Channels[i] = new ChannelServer((byte)i, port);
                port++;
            }

            m_loginPool = new List<string>();

        }

        public byte LoginClient(MapleClient c,string user,string pass)
        {
            lock(m_loginPool)
            {
                if (m_loginPool.Contains(user.ToLower()))
                    return 7; //already logged in

                var result = Database.Instance.Login(c, user, pass);

                if (result == 0)
                {
                    m_loginPool.Add(user);
                    c.LoggedIn = true;
                }

                return result;
            }
        }
        public void RemoveClient(string username)
        {
            lock (m_loginPool)
                m_loginPool.Remove(username.ToLower());
        }

        public void Run()
        {
            Login.Run();

            foreach (var c in Channels)
                c.Run();

            Running = true;

            MainForm.Instance.Log("RajanMS is online");
        }
        public void Shutdown()
        {
            MainForm.Instance.Log("Shutting down MasterServer");

            Login.Shutdown();

            foreach (var c in Channels)
                c.Shutdown();

            Running = false;

            MainForm.Instance.Log("RajanMS is offline");
        }
    }
}
