namespace RajanMS.GUI
{
    partial class RegisterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelUser = new System.Windows.Forms.Label();
            this.labelPw = new System.Windows.Forms.Label();
            this.labelPIC = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.txtPIC = new System.Windows.Forms.TextBox();
            this.chkbGM = new System.Windows.Forms.CheckBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Location = new System.Drawing.Point(12, 15);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(61, 13);
            this.labelUser.TabIndex = 0;
            this.labelUser.Text = "Username :";
            // 
            // labelPw
            // 
            this.labelPw.AutoSize = true;
            this.labelPw.Location = new System.Drawing.Point(12, 41);
            this.labelPw.Name = "labelPw";
            this.labelPw.Size = new System.Drawing.Size(59, 13);
            this.labelPw.TabIndex = 1;
            this.labelPw.Text = "Password :";
            // 
            // labelPIC
            // 
            this.labelPIC.AutoSize = true;
            this.labelPIC.Location = new System.Drawing.Point(12, 67);
            this.labelPIC.Name = "labelPIC";
            this.labelPIC.Size = new System.Drawing.Size(30, 13);
            this.labelPIC.TabIndex = 2;
            this.labelPIC.Text = "PIC :";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(87, 12);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(100, 20);
            this.txtUser.TabIndex = 3;
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(87, 38);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(100, 20);
            this.txtPass.TabIndex = 4;
            // 
            // txtPIC
            // 
            this.txtPIC.Location = new System.Drawing.Point(87, 64);
            this.txtPIC.Name = "txtPIC";
            this.txtPIC.Size = new System.Drawing.Size(100, 20);
            this.txtPIC.TabIndex = 5;
            // 
            // chkbGM
            // 
            this.chkbGM.AutoSize = true;
            this.chkbGM.Location = new System.Drawing.Point(12, 94);
            this.chkbGM.Name = "chkbGM";
            this.chkbGM.Size = new System.Drawing.Size(43, 17);
            this.chkbGM.TabIndex = 6;
            this.chkbGM.Text = "GM";
            this.chkbGM.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(112, 90);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 7;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(202, 117);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.chkbGM);
            this.Controls.Add(this.txtPIC);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.labelPIC);
            this.Controls.Add(this.labelPw);
            this.Controls.Add(this.labelUser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RegisterForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.Label labelPw;
        private System.Windows.Forms.Label labelPIC;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.TextBox txtPIC;
        private System.Windows.Forms.CheckBox chkbGM;
        private System.Windows.Forms.Button btnCreate;
    }
}