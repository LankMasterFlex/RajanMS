using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using RajanMS.Game;
using RajanMS.Servers;

namespace RajanMS
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        private delegate void LogDelegate(string fmt, params object[] args);
        private readonly LogDelegate m_logFunc;

        public MainForm()
        {
            Instance = this;
         
            InitializeComponent();
            m_logFunc = new LogDelegate(Log);

            Text += string.Concat(" v",Constants.MajorVersion, '.', Constants.MinorVersion);

            LoadConfig();
        }

        public void LoadConfig()
        {
            ConfigReader cr = new ConfigReader(Constants.ConfigName);

            int worlds = cr.ReadInt("Server", "Worlds");
            int channels = cr.ReadInt("Server", "Channels");

            MasterServer.Instance = new MasterServer(worlds, (short)channels);
        }

        public void Log(string fmt, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(m_logFunc, fmt, args);
            }
            else
            {
                string header = DateTime.Now.ToString("HH:mm:ss ");
                string final = string.Format(fmt, args);

                if (textBoxLog.Lines.Length > 100)
                    textBoxLog.Clear();

                textBoxLog.AppendText(header);
                textBoxLog.AppendText(final);
                textBoxLog.AppendText(Environment.NewLine);
                textBoxLog.ScrollToCaret();
            }
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            btnToggle.Enabled = false;
            MasterServer.Instance.Run();
            
            /*
            Account rajan = new Account()
            {
                AccountId = 0,
                Username = "rajan",
                Password = "12345",
                PIC = string.Empty,
                LastIP = string.Empty,
                LoggedIn = false,
                GM = false,
                Banned = false,
                BanReason = string.Empty,
            };

            Account arun = new Account()
            {
                AccountId = 0,
                Username = "arun",
                Password = "12345",
                PIC = string.Empty,
                LastIP = string.Empty,
                LoggedIn = false,
                GM = true,
                Banned = false,
                BanReason = string.Empty,
            };
            MasterServer.Instance.Database.SaveAccount(rajan);
            MasterServer.Instance.Database.SaveAccount(arun);
             */
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to close RajanMS?", "Server Shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (dr == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
            else if (dr == System.Windows.Forms.DialogResult.Yes)
            {

                MasterServer.Instance.Shutdown();
            }
        }
    }
}
