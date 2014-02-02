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
using RajanMS.Game;
using RajanMS.GUI;
using RajanMS.Servers;
using RajanMS.Tools;

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

            Text = string.Concat("RajanMS v", Constants.MajorVersion, '.', Constants.MinorVersion);

            MasterServer.Instance = new MasterServer();
        }


        public void Log(string fmt, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(m_logFunc, fmt, args);
            }
            else
            {
                string message = string.Concat(DateTime.Now.ToString("[HH:mm:ss] "),
                                               string.Format(fmt, args),
                                               Environment.NewLine);

                if (textBoxLog.Lines.Length > 100)
                    textBoxLog.Clear();

                textBoxLog.AppendText(message);
                textBoxLog.ScrollToCaret();
            }
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (btnToggle.Text == "Start")
            {
                MasterServer.Instance.Run();
                btnToggle.Text = "Shutdown";
            }
            else
            {
                var dr = MessageBox.Show("Are you sure you want to shutdown?", "Server Shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (dr == DialogResult.Yes)
                {
                    MasterServer.Instance.Shutdown();
                    btnToggle.Text = "Start";
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MasterServer.Instance.Running)
            {
                Log("Please shutdown the server first");
                e.Cancel = true;
            }
        }

        private void btnCreateAcc_Click(object sender, EventArgs e)
        {
            using (var gui = new RegisterForm())
                gui.ShowDialog();
        }
    }
}
