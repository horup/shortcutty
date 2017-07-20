using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shortcutty
{
   static class Program
   {
      static Form1 form;
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.SetCompatibleTextRenderingDefault(false);
         Application.EnableVisualStyles();
         form = new Form1();
         if (Directory.Exists(form.RootPath))
         {
            var kh = new KeyboardHook(true);
            kh.KeyDown += Kh_KeyDown;
            Application.Run(form);
         }
         else
         {
            MessageBox.Show(form.RootPath + " directory was not found. Please create this directory and place shortcuts before proceeding.");
         }
      }

      private static void Kh_KeyDown(Keys key, bool Shift, bool Ctrl, bool Alt)
      {

         if ((key == Keys.Oemtilde || key == Keys.Oem5) && Ctrl)
         {
            if (form.Visible)
               form.Hide();
            else
            {
               form.Show();
               form.Focus();
               form.Reset();
            }
         }
      }
   }
}
