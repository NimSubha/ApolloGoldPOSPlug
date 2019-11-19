/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// A helper class for accessing the 'user32.dll' native methods.
    /// </summary>
    public static class SafeNativeMethodsHelper
    {
        public static void BlockInput(bool blockIt)
        {
            SafeNativeMethods.BlockInput(blockIt);
        }

        public static int FindWindow(string className, string windowName)
        {
            return SafeNativeMethods.FindWindow(className, windowName).ToInt32();
        }

        public static int SetForegroundWindow(int handle)
        {
            return SafeNativeMethods.SetForegroundWindow(new IntPtr(handle));
        }

        public static bool ShowWindow(int handle, ShowWindowCommands command)
        {
            return SafeNativeMethods.ShowWindow(handle, command);
        }

        public static void BlockMouseAndKeyboard(bool block)
        {
            SafeNativeMethods.BlockMouseAndKeyboard(block);
        }

        private static class SafeNativeMethods
        {
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern void BlockInput([In, MarshalAs(UnmanagedType.Bool)]bool blockIt);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindow(String className, String windowName);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int SetForegroundWindow(IntPtr handle);

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(int handle, ShowWindowCommands command);

            public static void BlockMouseAndKeyboard(bool blockIt)
            {
#if !DEBUG
                blockIt = false;
#endif
                BlockInput(blockIt);
            }
        }
    }

    public enum ShowWindowCommands
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or 
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window 
        /// for the first time.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>       
        ShowMaximized = 3,
        /// <summary>
        /// Displays a window in its most recent size and position. This value 
        /// is similar to <see cref="ShowWindowCommands.Normal"/>, except 
        /// the window is not activated.
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position. 
        /// </summary>
        Show = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level 
        /// window in the Z order.
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="ShowWindowCommands.ShowMinimized"/>, except the 
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is 
        /// similar to <see cref="ShowWindowCommands.Show"/>, except the 
        /// window is not activated.
        /// </summary>
        ShowNa = 8,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or 
        /// maximized, the system restores it to its original size and position. 
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Sets the show state based on the SW_* value specified in the 
        /// STARTUPINFO structure passed to the CreateProcess function by the 
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
        /// that owns the window is not responding. This flag should only be 
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }

}