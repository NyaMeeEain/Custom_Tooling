using System;
using System.Diagnostics;
using System.IO;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputPath = @"C:\Users\Public\output.txt";

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                RunCommand("net users", writer);
                RunCommand("systeminfo", writer);
                RunCommand("netstat -ant", writer);
                RunCommand("wmic qfe list brief", writer);
                RunCommand("wmic computersystem list full", writer);
                RunCommand(@"wmic /namespace:\\root\securitycenter2 path antivirusproduct GET displayName, productState, pathToSignedProductExe", writer);
                RunCommand("cmd /c wmic service get name,displayname,pathname,startmode |findstr /i \"auto\" |findstr /i /v \"c:\\windows\\\\\" |findstr /i /v \"\"\"", writer);
            }

            Console.WriteLine($"Output saved to: {outputPath}");
            Console.WriteLine("Press any key to exit...");
           
        }

        static void RunCommand(string command, StreamWriter writer)
        {
            writer.WriteLine($"Timestamp: {DateTime.Now}");
            writer.WriteLine($"Command: {command}");

            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();

            // Execute the command
            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit");
            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();

            process.WaitForExit();

            writer.WriteLine("Output:");
            writer.WriteLine(output);
            writer.WriteLine("Errors:");
            writer.WriteLine(errors);
            writer.WriteLine(new string('-', 80));
        }
    }
}
