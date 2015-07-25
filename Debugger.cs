using System;
using System.Diagnostics;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HWND = System.IntPtr;
using System.Text;

namespace Rocket.Unturned
{
    public class Debugger : MonoBehaviour
    {

        public static IDictionary<HWND, string> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        private DateTime lastUpdate = DateTime.Now;
        private byte maxPlayers;

        public void Awake()
        {
            if (GetOpenWindows().Where(k => k.Value == "Select Unity Instance").Count() == 0)
            {
                U.Initialize();
                Destroy(this);
                return;
            }
            maxPlayers = SDG.Unturned.Steam.MaxPlayers;
            SDG.Unturned.Steam.MaxPlayers = 0;
            Console.Write("Waiting for debugger...");
        }

        public void FixedUpdate()
        {
            if ((DateTime.Now - lastUpdate).TotalSeconds > 3)
            {
                Console.Write(".");
                lastUpdate = DateTime.Now;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("\nDebugger found, continuing...");
                SDG.Unturned.Steam.MaxPlayers = maxPlayers;
                U.Initialize();
                Destroy(this);
            }
        }
    }
}