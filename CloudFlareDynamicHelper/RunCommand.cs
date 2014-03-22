using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CloudFlareDynamicHelper
{
    class RunCommand
    {
        System.Diagnostics.Process pro;

        public RunCommand(String cmd)
        {
            pro = new Process();

            pro.StartInfo.FileName = System.Environment.CurrentDirectory + "\\lib\\node.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.Arguments = "lib\\json.js " + cmd;
            pro.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        }

        public String Run()
        {
            String Text = "";
            pro.Start();
            string output = pro.StandardOutput.ReadToEnd();
            if (String.IsNullOrEmpty(output) == false) Text = output;
            pro.WaitForExit();
            pro.Close();

            return Text;
        }
    }
}
