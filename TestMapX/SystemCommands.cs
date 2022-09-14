// //File: SystemCommands.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-14
// //
using System;
using System.Diagnostics;
namespace TestMapX
{
    public class SystemCommands
    {
        public SystemCommands()
        {
        }
        /// <span class="code-SummaryComment"><summary></span>
        /// Executes a shell command synchronously.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="command">string command</param></span>
        /// <span class="code-SummaryComment"><returns>string, as output of the command.</returns></span>
        public string RunCommand(string command, string args)
        {
            var output = "";
            try
            {
                var procStartInfo = new ProcessStartInfo(command, args)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    Arguments = args
                };

                var proc = new Process { StartInfo = procStartInfo };
                proc.Start();

                // Get the output into a string
                output = proc.StandardOutput.ReadToEnd();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}\n{1}", e.Message, e.StackTrace);
                output = "FAILURE: Exception " + e.Message;
            }
            return output.ToString();

        }
    }
}
