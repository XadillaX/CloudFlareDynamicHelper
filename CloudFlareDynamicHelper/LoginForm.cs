using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using LitJson;
using System.IO;

namespace CloudFlareDynamicHelper
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            try
            {
                // 读入JSON
                FileStream aFile = new FileStream("token.json", FileMode.Open);
                StreamReader sr = new StreamReader(aFile);
                string json = sr.ReadToEnd();
                sr.Close();

                JsonData data = JsonMapper.ToObject(json);
                inputEmail.Text = (string)data["email"];
                inputToken.Text = (string)data["token"];
            }
            catch (Exception e)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RunCommand rc = new RunCommand("settoken -E" + inputEmail.Text + " -T" + inputToken.Text);
            String Text = rc.Run();

            JsonData data = JsonMapper.ToObject(Text);
            bool status = (bool)data["status"];

            if (status)
            {
                // 下一环节...
                DomainListForm dlf = new DomainListForm(this);
                this.Hide();
                dlf.Show();
            }
            else
            {
                MessageBox.Show("Failed: " + (string)data["msg"], "CloudFlare Dynamic Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void inputToken_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
