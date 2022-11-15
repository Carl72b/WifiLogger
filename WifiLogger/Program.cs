using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace WifiLogger
{
    internal class Program
    {
        static void ProcessOutput(string output, string path)
        {
            var lines = output.Split('\n');

            var xparams = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                var pos = line.IndexOf(" : ");

                if (pos == -1)
                    continue;

                var key = line.Substring(0, pos).Trim();
                var val = line.Substring(pos + 2).Trim();

                xparams[key] = val;
            }

            var items = new List<string>();

            var fields = new List<string>() { 
                "Physical address",
                "SSID",
                "BSSID",
                "Radio type",
                "Authentication",
                "Cipher",
                "Band",
                "Channel",
                "Receive rate (Mbps)",
                "Transmit rate (Mbps)",
                "Signal",
            };

            items.Add(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

            foreach (var field in fields)
            {
                items.Add(xparams[field]);
            }

            var file_line = String.Join(",", items);

            Console.WriteLine(file_line);

            System.IO.File.AppendAllText(
                path,
                file_line + "\n"
            );
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine(
                    "Specifiy the path for the output file as a argument to this program."
                );

                return;
            }

            while (true)
            {
                var p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "C:\\Windows\\System32\\netsh.exe";
                p.StartInfo.Arguments = "wlan show interfaces";
                p.Start();
                var output = p.StandardOutput.ReadToEnd();
                ProcessOutput(output, args[0]);
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
