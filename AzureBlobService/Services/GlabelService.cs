using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using AzureBlobService;

namespace AzureBlobService.Services
{
    internal class GLabelService
    {
        internal static void SendToPrinter(string printerName, string data, string name)
        {
            var dict = new Dictionary<string, string>();

            foreach (var s in data.Split('\n'))
            {
                var split = s.Trim().Split('=');
                var val = split[1];
                for (var i = 2; i < split.Length; i++)
                {
                    val += "=" + split[i];
                }
                    
                dict.Add(split[0].Trim(), val.Trim());
            }

            Config config = new Config();
            if (string.IsNullOrEmpty(config.GlabelsPath))
                throw new Exception("Glabels not found");

            var parms = new StringBuilder();
            parms.Append($"-p \"{printerName}\"");
            foreach(var k in dict)
            {
                if (k.Key == "LabelTemplate" || k.Key == "Printer" || k.Key == "Copies")
                    continue;
                parms.Append($" -D \"{k.Key}\"=\"{k.Value}\"");
            }
            parms.Append($" {config.TemplatePath}\\{dict["LabelTemplate"]}");

            var procinfo = new ProcessStartInfo(config.GlabelsPath, parms.ToString());
            procinfo.UseShellExecute = false;
            procinfo.RedirectStandardError = true;
            procinfo.RedirectStandardOutput = true;
            using (var q = Process.Start(procinfo))
            {
                using (StreamReader reader = q.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
                using (StreamReader reader = q.StandardError)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
                q.WaitForExit();
            }
        }

        private static void Q_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private static void Q_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
