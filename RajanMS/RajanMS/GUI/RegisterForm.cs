using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RajanMS.Game;
using RajanMS.Servers;
using RajanMS.Tools;

namespace RajanMS.GUI
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text) || txtPass.Text.Length < 5)
            {
                MessageBox.Show("User is empty or password length is < 5");
                return;
            }

            Account target = new Account()
            {
                AccountId = MasterServer.Instance.Database.GetNewAccountId(),
                Username = txtUser.Text,
                Password = txtPass.Text,
                PIC = txtPIC.Text,

                LastIP = string.Empty,

                Gender = 0,

                LoggedIn = false,
                GM = chkbGM.Checked,

                Banned = false,
                BanReason = string.Empty
            };

            MasterServer.Instance.Database.Save<Account>(Database.Accounts,target);

            Close(); //work is done here fam
        }
    }
}
