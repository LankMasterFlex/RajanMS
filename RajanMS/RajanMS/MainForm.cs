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
            MasterServer.Instance.Run();
            btnToggle.Enabled = false;
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
