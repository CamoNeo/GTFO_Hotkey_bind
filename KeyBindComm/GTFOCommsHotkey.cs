using System;
using System.Runtime.InteropServices;
using System.Threading;

public static class GTFOCommsHotkey
{
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_ALT = 0x0001;
    private const int HOTKEY_ID = 1;
    private const int VK_B = 0x42; // 'B' key
    private const int MOD_NONE = 0x0000; // No modifiers

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private const uint KEYEVENTF_KEYDOWN = 0;
    private const uint KEYEVENTF_KEYUP = 2;

    public static void RegisterCommsHotkey(IntPtr windowHandle)
    {
        // B key for comms
        if (!RegisterHotKey(windowHandle, HOTKEY_ID, MOD_NONE, VK_B))
        {
            var err = Marshal.GetLastWin32Error();
            Console.WriteLine($"Failed to register hotkey (error {err}). Make sure another app hasn't already registered it.");
        }
    }

    public static void UnregisterCommsHotkey(IntPtr windowHandle)
    {
        UnregisterHotKey(windowHandle, HOTKEY_ID);
    }

    public static void HandleCommsHotkey()
    {
        Console.WriteLine("GTFO Comms activated - B");
        SendKeySequence("q 1 1");
    }

    private static void SendKey(byte vKey)
    {
        keybd_event(vKey, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        Thread.Sleep(25);
        keybd_event(vKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        Thread.Sleep(25);
    }

    private static void SendKeySequence(string sequence)
    {
        byte firstKeyVk = 0;
        bool isFirstKey = true;

        foreach (char c in sequence)
        {
            byte vKey = 0;

            if (char.IsLetter(c))
                vKey = (byte)char.ToUpper(c);
            else if (char.IsDigit(c))
                vKey = (byte)((c - '0' == 0) ? 0x30 : (c - '0' + 0x30));
            else if (c == ' ')
                continue; // Skip spaces
            else
                continue;

            if (isFirstKey)
            {
                firstKeyVk = vKey;
                keybd_event(firstKeyVk, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                isFirstKey = false;
                Thread.Sleep(25);
            }
            else
            {
                // Send other keys while holding first key
                keybd_event(vKey, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                Thread.Sleep(25);
                keybd_event(vKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                Thread.Sleep(25);
            }
        }

        // Release the first key at the end
        if (firstKeyVk != 0)
        {
            keybd_event(firstKeyVk, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
