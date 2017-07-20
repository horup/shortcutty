using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shortcutty
{
   public partial class Form1 : Form
   {
      private int blinks = 0;
      private string filter = "";
      private DirectoryInfo root;
      private DirectoryInfo current;
      private Font f = new Font("Consolas", 24, FontStyle.Regular, GraphicsUnit.Pixel);
      private int line = 0;

      public Form1()
      {
         InitializeComponent();
      }

    

      public string RootPath
      {
         get
         {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SHORTCUTTY";
         }
      }
      public void Reset()
      {
         this.root = new DirectoryInfo(this.RootPath);
         this.current = this.root;
         this.Filter = "";
         this.Invalidate();
      }

      private string Filter
      {
         get
         {
            return this.filter;
         }
         set
         {
            this.filter = value;
            this.blinks = 0;
            this.blinker.Stop();
            this.blinker.Start();
            this.Line = 0;
         }
      }

      private DirectoryInfo Current
      {
         get
         {
            return this.current;
         }
         set
         {
            this.current = value;
            this.Invalidate();
         }
      }

      private int Line
      {
         get
         {
            return this.line;
         }
         set
         {
            this.line = value;
            this.Invalidate();
         }
      }

      private void PaintLine(string s, Graphics g, int lineNum, Icon icon)
      {
         var offset = 1;
         var brush = Brushes.White;
         if (line == lineNum)
         {
            g.FillRectangle(Brushes.White, 0, (lineNum + offset) * f.Height, g.ClipBounds.Width, f.Height);
            brush = Brushes.Black;
         }

         if (icon != null)
         {
            g.DrawIcon(icon, new Rectangle(0, (lineNum + offset) * f.Height, f.Height, f.Height));
         }

         g.DrawString(s, this.f, brush, f.Height, (lineNum + offset) * f.Height);

      }

      private List<FileSystemInfo> FileSystem
      {
         get
         {
            var files = this.Current.EnumerateFiles() as IEnumerable<FileSystemInfo>;
            var list = new List<FileSystemInfo>();

            if (filter.Length == 0)
            {
               list = files.ToList();
            }
            else
            {
               foreach (var f in files)
               {
                  var found = f.Name.Length >= filter.Length && f.Name.ToUpper().Substring(0, filter.Length) == filter.ToUpper();
                  if (found)
                  {
                     list.Add(f);
                  }
               }

               foreach (var f in files)
               {
                  var found = !list.Any((ff) => ff.Name == f.Name) && f.Name.ToUpper().Contains(filter.ToUpper());
                  if (found)
                  {
                     list.Add(f);
                  }
               }
            }

            return list;
         }
      }

      private void MoveLine(int dir)
      {
         line += dir;
         if (line < 0)
            line = 0;
         else if (line >= this.FileSystem.Count)
         {
            line = this.FileSystem.Count - 1;
         }

         this.Invalidate();
      }

      private void DoneOK()
      {
         this.Enabled = true;
      }

      private void OK()
      {
         try
         {
            var selected = this.FileSystem[this.line];
            if (selected is DirectoryInfo)
            {
               var dir = selected as DirectoryInfo;
               this.Current = dir;
               this.line = 0;
            }
            else if (selected is FileInfo)
            {
               Task t = new Task(new Action(() =>
               {
                  Process.Start(selected.FullName);
                  this.BeginInvoke(new Action(() => this.DoneOK()));
               }));

               t.Start();
               this.Hide();
            }
         }
         catch (Exception)
         {

         }
      }

      private void blinker_Tick(object sender, EventArgs e)
      {
         this.Invalidate();
         this.blinks++;
      }
     
      protected override void OnKeyPress(KeyPressEventArgs e)
      {
         base.OnKeyPress(e);
         if (Char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == ' ')
         {
            this.Filter += e.KeyChar;
            this.Filter = this.Filter.TrimStart();
         }
      }
      protected override void OnShown(EventArgs e)
      {
         base.OnShown(e);
         this.Reset();
      }
      protected override void OnKeyDown(KeyEventArgs e)
      {
         base.OnKeyDown(e);
         switch (e.KeyCode)
         {
            case Keys.Down:
               MoveLine(1);
               break;
            case Keys.Up:
               MoveLine(-1);
               break;
            case Keys.Return:
               this.OK();
               break;
            case Keys.Back:
               if (this.Filter.Length > 0)
                  this.Filter = this.Filter.Substring(0, this.Filter.Length - 1);
               break;
            case Keys.Oem6:
            case Keys.Oemtilde:
               this.Reset();
               break;
         }
      }

      protected override void OnDeactivate(EventArgs e)
      {
         base.OnDeactivate(e);
         this.Hide();
      }

      protected override void OnMouseClick(MouseEventArgs e)
      {
         base.OnMouseClick(e);
         this.OK();
      }

      private Dictionary<string, Icon> icons = new Dictionary<string, Icon>();

      private Icon GetIcon(string path)
      {
         if (!icons.ContainsKey(path))
         {
            icons[path] = System.Drawing.Icon.ExtractAssociatedIcon(path);
         }

         return icons[path];
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);
         var g = e.Graphics;
         g.Clear(Color.Black);
         int i = 0;
         var system = FileSystem;


         foreach (var dir in system)
         {
            Icon icon = GetIcon(dir.FullName);
            this.PaintLine(Path.GetFileNameWithoutExtension(dir.FullName).ToUpper(), g, i++, icon);
         }

         var dim = g.MeasureString(this.Filter, this.f);
         g.DrawString(this.Filter.ToUpper(), this.f, Brushes.DarkGray, this.f.Height, 0 * f.Height);
         g.DrawString(">>", this.f, Brushes.DarkGray, -3, 0);
         if (this.blinks % 2 == 0)
         {
            g.FillRectangle(Brushes.DarkGray, dim.Width + this.f.Height, 0, 2, this.f.Height);
         }
      }

      protected override void OnMouseWheel(MouseEventArgs e)
      {
         base.OnMouseWheel(e);
         MoveLine(e.Delta > 0 ? -1 : 1);

      }
      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         this.Cursor = Cursors.Default;
         Cursor.Show();
         if (MessageBox.Show("Are you sure?", "Close", MessageBoxButtons.YesNo) == DialogResult.No)
         {
            e.Cancel = true;
            this.Show();
            this.Activate();
         }

         base.OnFormClosing(e);
      }

      protected override void OnVisibleChanged(EventArgs e)
      {
         if (this.Visible)
         {
            uint foreThread = Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), IntPtr.Zero);
            uint appThread = Win32.GetCurrentThreadId();
            const uint SW_SHOW = 5;
            if (foreThread != appThread)
            {
               Win32.AttachThreadInput(foreThread, appThread, true);
               Win32.BringWindowToTop(this.Handle);
               Win32.ShowWindow(this.Handle, SW_SHOW);
               Win32.AttachThreadInput(foreThread, appThread, false);
            }
            else
            {
               Win32.BringWindowToTop(this.Handle);
               Win32.ShowWindow(this.Handle, SW_SHOW);
            }
            this.Activate();

            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
            Cursor = Cursors.Cross;
         }
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);
         this.Hide();
      }
   }
}
