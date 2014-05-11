using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LitJson;

namespace CloudFlareDynamicHelper
{
    public partial class DomainListForm : Form
    {
        LoginForm LF;
        Boolean ShowLogin = true;

        public DomainListForm(LoginForm lf)
        {
            LF = lf;
            InitializeComponent();
        }

        private void DomainListForm_Shown(object sender, EventArgs e)
        {
            // 获取域名列表
            RunCommand rc = new RunCommand("loadDomains");
            String Result = rc.Run();
            JsonData data;

            try
            {
                data = JsonMapper.ToObject(Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CloudFlare Dynamic Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (!(bool)data["status"])
            {
                MessageBox.Show((String)data["msg"], "CloudFlare Dynamic Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            data = data["result"];
            if ((string)data["result"] != "success")
            {
                MessageBox.Show((String)data["msg"], "CloudFlare Dynamic Helper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            ShowLogin = false;

            // 载入所有域名下的子域名
            data = data["response"]["zones"];
            for (int i = 0; i < (int)data["count"]; i++)
            {
                JsonData domain = data["objs"][i];
                String zonename = (string)domain["zone_name"];

                // 载入子域名的记录
                LoadRecord(zonename);
            }

            LogField.AppendText("\nAll loaded...\n");
        }

        private void LoadRecord(String domain)
        {
            RunCommand rc = new RunCommand("loadRecord -D" + domain);
            String Result = rc.Run();

            JsonData data;
            try
            {
                data = JsonMapper.ToObject(Result);
            }
            catch (Exception ex)
            {
                LogField.AppendText("\nError while loading records of " + domain + ": " + ex.Message);
                return;
            }

            if (!(bool)data["status"])
            {
                LogField.AppendText("\nError while loading records of " + domain + ": " + (String)data["msg"]);
                return;
            }

            data = data["result"];
            if ((string)data["result"] != "success")
            {
                LogField.AppendText("\nError while loading records of " + domain + ": " + (String)data["msg"]);
                return;
            }

            data = data["response"]["recs"];
            for (int i = 0; i < (int)data["count"]; i++)
            {
                JsonData rec = data["objs"][i];
                String rec_id = (string)rec["rec_id"];
                String zone_name = (string)rec["zone_name"];
                String display_name = (string)rec["display_name"];
                String type = (string)rec["type"];
                String content = (string)rec["content"];

                // 只记录A记录
                if (type == "A")
                {
                    ValidRecord vr = new ValidRecord(rec_id, display_name, zone_name, type, content);

                    int index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = rec_id;
                    dataGridView1.Rows[index].Cells[1].Value = false;
                    dataGridView1.Rows[index].Cells[2].Value = display_name;
                    dataGridView1.Rows[index].Cells[3].Value = zone_name;
                    dataGridView1.Rows[index].Cells[4].Value = content;
                }
            }
        }

        private void DomainListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ShowLogin)
            {
                LF.Show();
            }
            else
            {
                LF.Close();
            }
        }

        private void DomainListForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start listening (&S)")
            {
                button1.Text = "Stop listening (&S)";
                timer1.Enabled = true;
            }
            else
            {
                button1.Text = "Start listening (&S)";
                timer1.Enabled = false;
            }
        }

        private String GetIp()
        {
            RunCommand rc = new RunCommand("getip");
            String text = rc.Run();

            while (text[text.Length - 1] == '\r' || text[text.Length - 1] == '\n')
            {
                text = text.Substring(0, text.Length - 1);
            }

            return text;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // ip...
            String ip = GetIp();

            // if ip is `0.0.0.0` then return...
            if (ip == "0.0.0.0") return;

            // all selected domains...
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Boolean)dataGridView1.Rows[i].Cells[1].Value)
                {
                    String rec_id = (String)dataGridView1.Rows[i].Cells[0].Value;
                    String display_name = (String)dataGridView1.Rows[i].Cells[2].Value;
                    String zone_name = (String)dataGridView1.Rows[i].Cells[3].Value;
                    String content = (String)dataGridView1.Rows[i].Cells[4].Value;

                    if (content != ip)
                    {
                        String domain_name = display_name + "." + zone_name;
                        if (display_name == zone_name)
                        {
                            domain_name = zone_name;
                            display_name = "@";
                        }

                        LogField.AppendText("== " + domain_name + " =='s ip (" + content + ") is different with current ip (" + ip + ").\n");
                        LogField.ScrollToCaret();

                        SetRecordDelegate invoker = new SetRecordDelegate(SetRecord);
                        invoker.BeginInvoke(rec_id, domain_name, zone_name, display_name, ip, i, null, null);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = false;
            }
        }

        delegate void SetRecordDelegate(String id, String display, String domain, String name, String ip, int idx);
        private void SetRecord(String id, String display, String domain, String name, String ip, int idx)
        {
            RunCommand rc = new RunCommand("editRecord -ID" + id + " -D" + domain + " -N" + name + " -IP" + ip);
            String Result = rc.Run();

            JsonData data;
            try
            {
                data = JsonMapper.ToObject(Result);
            }
            catch (Exception ex)
            {
                LogField.AppendText("\nError while editing records: " + ex.Message);
                return;
            }

            if (!(bool)data["status"])
            {
                LogField.AppendText("\nError while editing records: " + (String)data["msg"]);
                return;
            }

            data = data["result"];
            if ((string)data["result"] != "success")
            {
                LogField.AppendText("\nError while editing records: " + (String)data["msg"]);
                return;
            }

            data = data["response"]["rec"]["obj"];

            dataGridView1.Rows[idx].Cells[4].Value = (string)data["content"];
            LogField.AppendText("-- " + display + " -- has already changed to (" + ip + ").\n");
        }
    }
}
