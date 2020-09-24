/* Any copyright is dedicated to the Public Domain.
 * https://creativecommons.org/publicdomain/zero/1.0/ */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Wowpaper
{
    /// <summary>
    /// Contains methods and extensions that provide
    /// ability to set a WinForm as system wallpaper
    /// </summary>
    public static class WallpaperManager
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lclassName, string windowTitle);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [Flags]
        private enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
    IntPtr hWnd,
    uint Msg,
    UIntPtr wParam,
    IntPtr lParam,
    SendMessageTimeoutFlags fuFlags,
    uint uTimeout,
    out UIntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result);

        private static int GWL_STYLE = -16;
        private static int WS_CHILD = 0x40000000;

        public static IntPtr GetSysListWindow()
        {
            IntPtr hwnd = FindWindow("Progman", null);
            if (hwnd != IntPtr.Zero) hwnd = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (hwnd != IntPtr.Zero) hwnd = FindWindowEx(hwnd, IntPtr.Zero, "SysListView32", null);

            return hwnd;
        }

        public static IntPtr GetWallpaperWorkerWindow()
        {
            IntPtr progman = FindWindow("Progman", null);
            IntPtr result;
            SendMessageTimeout(progman,
                       0x052C,
                       new IntPtr(0),
                       IntPtr.Zero,
                       SendMessageTimeoutFlags.SMTO_NORMAL,
                       1000,
                       out result);

            IntPtr workerw = IntPtr.Zero;
            EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            null);

                if (p != IntPtr.Zero)
                    workerw = FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               null);

                return true;
            }), IntPtr.Zero);

            return workerw;
        }

        public static void Reparent(IntPtr guestHandle, IntPtr hostHandle)
        {
            SetWindowLong(guestHandle, GWL_STYLE, GetWindowLong(guestHandle, GWL_STYLE) | WS_CHILD);
            SetParent(guestHandle, hostHandle);
        }

        /// <summary>
        /// Replace the Windows wallpaper with form
        /// </summary>
        /// <param name="form">the form to set as wallpaper</param>
        public static void SetAsWallpaper(this Form form)
        {
            form.Hide();

            form.FormBorderStyle = FormBorderStyle.None;

            form.Bounds = Screen.PrimaryScreen.Bounds;
            form.SetBounds(0, 0, 0, 0, BoundsSpecified.Location);

            var wp = GetWallpaperWorkerWindow();
            Reparent(form.Handle, wp);

            form.Show();
        }
    }
}
