using System;
using System.Windows.Forms;

namespace RajanMS
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /* My config file
            Worlds=1
            Channels=1
            Host=mongodb://localhost
            Name=RajanMS
         */
    }
}
