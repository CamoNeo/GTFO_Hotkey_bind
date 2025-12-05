using System;
using System.Runtime.InteropServices;

internal static class Program
{
    private const int WM_HOTKEY = 0x0312;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x; public int y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
    }

    [DllImport("user32.dll")]
    private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

    private static void Main()
    {
        Console.WriteLine("Registering hotkey Ctrl+Alt+C (press Ctrl+C in console to exit)...");

        // Register for the current thread (hWnd = IntPtr.Zero)
        GTFOCommsHotkey.RegisterCommsHotkey(IntPtr.Zero);

        try
        {
            MSG msg;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            {
                if (msg.message == WM_HOTKEY)
                {
                    GTFOCommsHotkey.HandleCommsHotkey();
                }

                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }
        finally
        {
            GTFOCommsHotkey.UnregisterCommsHotkey(IntPtr.Zero);
            Console.WriteLine("Unregistered hotkey and exiting.");
        }
    }
}
