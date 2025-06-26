using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EffyDocOutlookAddin.Services;

namespace EffyDocOutlookAddin.UI
{
    public partial class LoginForm : Form
    {
        private ApiService apiService;

        public LoginForm(ApiService apiService)
        {
            InitializeComponent();
            this.apiService = apiService;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both email and password.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Logging in...";
                lblStatus.Text = "Authenticating...";
                lblStatus.ForeColor = Color.Blue;

                var loginResponse = await apiService.LoginAsync(txtEmail.Text, txtPassword.Text);
                
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.access_token))
                {
                    lblStatus.Text = "Login successful!";
                    lblStatus.ForeColor = Color.Green;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.Text = "Login failed - Invalid response";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Login failed";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Login error: {ex.Message}", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtEmail.Focus();
        }
    }

    public partial class LoginForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblEmail;
        private Label lblPassword;
        private Label lblStatus;
        private PictureBox pictureBoxLogo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtEmail = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnCancel = new Button();
            this.lblEmail = new Label();
            this.lblPassword = new Label();
            this.lblStatus = new Label();
            this.pictureBoxLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            
            // pictureBoxLogo
            this.pictureBoxLogo.Location = new Point(12, 12);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new Size(64, 64);
            this.pictureBoxLogo.TabIndex = 0;
            this.pictureBoxLogo.TabStop = false;
            
            // lblEmail
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new Point(90, 25);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new Size(35, 13);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "Email:";
            
            // txtEmail
            this.txtEmail.Location = new Point(140, 22);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new Size(200, 20);
            this.txtEmail.TabIndex = 2;
            
            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new Point(90, 55);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(56, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            
            // txtPassword
            this.txtPassword.Location = new Point(140, 52);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(200, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.KeyPress += new KeyPressEventHandler(this.txtPassword_KeyPress);
            
            // btnLogin
            this.btnLogin.Location = new Point(185, 90);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(75, 25);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            
            // btnCancel
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(265, 90);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 25);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            
            // lblStatus
            this.lblStatus.Location = new Point(12, 125);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(328, 20);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Enter your effyDOC credentials";
            this.lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            
            // LoginForm
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new Size(352, 155);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.pictureBoxLogo);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "effyDOC Login";
            this.Load += new EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}