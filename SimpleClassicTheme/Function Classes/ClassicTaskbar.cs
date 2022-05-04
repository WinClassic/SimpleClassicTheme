/*
 *  SimpleClassicTheme, a basic utility to bring back classic theme to newer versions of the Windows operating system.
 *  Copyright (C) 2021 Anis Errais
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicTheme
{
    public static class ClassicTaskbar
    { 
        public static void FixWin8_1()
        {
            /*
             Remove taskbar blur
             */

            // Get the handle of the taskbar window
            IntPtr taskBarHandle = User32.FindWindowExW(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", "");
            // Create an ACCENTPOLICY structure which describes that all blur must be disabled
            User32.ACCENTPOLICY accentPolicy = new User32.ACCENTPOLICY { nAccentState = 0 };
            // Get the size of the ACCENTPOLICY structure
            int accentPolicySize = Marshal.SizeOf(accentPolicy);
            // Allocate unmanaged memory to accomodate for the ACCENTPOLICY structure
            IntPtr accentPolicyPtr = Marshal.AllocHGlobal(accentPolicySize);
            // Copy the struct to unmanaged memory so that a pointer to it can be given to Win32
            Marshal.StructureToPtr(accentPolicy, accentPolicyPtr, false);
            // Create a WINCOMPATTRDATA instance which sets the WindowCompositionAttribute (19) to the ACCENTPOLICY instance
            var winCompatData = new User32.WINCOMPATTRDATA
            {
                nAttribute = 19,
                ulDataSize = accentPolicySize,
                pData = accentPolicyPtr
            };
            // Tell Windows to apply the attribute
            User32.SetWindowCompositionAttribute(taskBarHandle, ref winCompatData);
            // Free the pointer to the ACCENTPOLICY instance
            Marshal.FreeHGlobal(accentPolicyPtr);

            /*
             Remove taskbar borders
             */

            // Get the current style of the taskbar window
            IntPtr p = User32.GetWindowLongPtrW(taskBarHandle, -16);
            // Add WS_DLGFRAME to the window style
            User32.SetWindowLongPtrW(taskBarHandle, -16, new IntPtr(p.ToInt64() | 0x400000));
            // Remove WS_DLGFRAME from the window style
            User32.SetWindowLongPtrW(taskBarHandle, -16, new IntPtr(p.ToInt64() ^ 0x400000));
        }

        public static void InstallSCTT(Form parent, bool ask = true)
		{
            if (!ask || CommonControls.TaskDialog.Show(parent, "Please note that SCTT is not being actively developed anymore, and support with issues will not be provided.", "Simple Classic Theme Taskbar", "Would you like to install Simple Classic Theme Taskbar?", CommonControls.TaskDialogButtons.Yes | CommonControls.TaskDialogButtons.No, CommonControls.TaskDialogIcon.WarningIcon) == DialogResult.Yes)
			{
                GithubDownloader download = new GithubDownloader(GithubDownloader.DownloadableGithubProject.SimpleClassicThemeTaskbar);
                download.ShowDialog(parent);
			}
		}

        public static void EnableSCTT()
		{
            Process[] scttInstances = Process.GetProcessesByName("SimpleClassicThemeTaskbar");
            scttInstances = scttInstances.Where(a =>
            {
                foreach (IntPtr handle in User32.EnumerateProcessWindowHandles(a.Id, "SCTT_Shell_TrayWnd"))
                {
                    IntPtr returnValue = User32.SendMessage(handle, User32.WM_SCT, new IntPtr(User32.SCTWP_ISSCT), IntPtr.Zero);
                    if (returnValue != IntPtr.Zero)
                        return true;
                }
                return false;
            }).ToArray();
            if (scttInstances.Length == 0)
                Process.Start($"{SCT.Configuration.InstallPath}Taskbar\\SimpleClassicThemeTaskbar.exe", "--sct");
		}
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassName (int hWnd, StringBuilder title, int size);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetWindowText(int hWnd, StringBuilder title, int size);
        public static void DisableSCTT()
		{
            Process[] scttInstances = Process.GetProcessesByName("SimpleClassicThemeTaskbar");
            Array.ForEach(scttInstances, a =>
            {
                List<IntPtr> handles = User32.EnumerateProcessWindowHandles(a.Id, "SCTT_Shell_TrayWnd");
                string s = "";
                foreach (IntPtr handle in handles)
                {
                    StringBuilder builder = new StringBuilder(1000);
                    GetClassName(handle.ToInt32(), builder, 1000);
                    if (builder.Length > 0)
                        s = s + builder.ToString() + "\n";
                    IntPtr returnValue = User32.SendMessage(handle, User32.WM_SCT, new IntPtr(User32.SCTWP_ISSCT), IntPtr.Zero);
                    if (returnValue != IntPtr.Zero)
                    {
                        User32.SendMessage(handle, User32.WM_SCT, new IntPtr(User32.SCTWP_EXIT), IntPtr.Zero);
                    }
                }
            });
        }
    }
}
