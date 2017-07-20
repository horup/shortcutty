using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shortcutty
{
   public static class Win32
   {
      [DllImport("user32.dll", SetLastError = true)]
      static public extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

      // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
      [DllImport("user32.dll")]
      static public extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

      [DllImport("kernel32.dll")]
      static public extern uint GetCurrentThreadId();

      /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
      [DllImport("user32.dll")]
      public static extern IntPtr GetForegroundWindow();

      [DllImport("user32.dll")]
      static public extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

      [DllImport("user32.dll", SetLastError = true)]
      static public extern bool BringWindowToTop(IntPtr hWnd);

      [DllImport("user32.dll", SetLastError = true)]
      static public extern bool BringWindowToTop(HandleRef hWnd);

      [DllImport("user32.dll")]
      static public extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
   }
}
