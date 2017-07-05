using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shortcutty
{
   public partial class Form1 : Form
   {
      DirectoryInfo root;
      DirectoryInfo current;

      public DirectoryInfo Current
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

      int line = 0;
      public int Line
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

      public Form1()
      {
         this.root = new DirectoryInfo(@"C:\Users\soh\Desktop\SHORTCUTTY");
         this.current = this.root;

         InitializeComponent();
      }
      Font f = new Font("Consolas", 16, FontStyle.Regular, GraphicsUnit.Pixel);
      void PaintLine(string s, Graphics g, int lineNum)
      {
         if (lineNum == 0)
         {
            s = "..";
         }
         if (line == lineNum)
         {
            g.FillRectangle(Brushes.White, 0, lineNum * f.Height, g.ClipBounds.Width, f.Height);
            g.DrawString(s, this.f, Brushes.Black, 0, lineNum * f.Height);
         }
         else
         {
            g.DrawString(s, this.f, Brushes.White, 0, lineNum * f.Height);
         }
      }

      private List<FileSystemInfo> FileSystem
      {
         get
         {
            var dirs = this.Current.EnumerateDirectories() as IEnumerable<FileSystemInfo>;
            var files = this.Current.EnumerateFiles() as IEnumerable<FileSystemInfo>;
            var list = new List<FileSystemInfo>();

            list.Add(this.current.Parent);
            list.AddRange(dirs);
            list.AddRange(files);
            return list;
         }
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
            this.PaintLine(dir.Name, g, i++);
         }

      }

      private void Form1_KeyPress(object sender, KeyPressEventArgs e)
      {
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

      private void OK()
      {
         try
         {
            var selected = this.FileSystem[this.line];
            if (selected is DirectoryInfo)
            {
               var dir = selected as DirectoryInfo;
               this.Current = dir;
            }
         }
         catch(Exception)
         {

         }
      }

      private void Back()
      {

      }

      private void Form1_KeyDown(object sender, KeyEventArgs e)
      {
         switch (e.KeyCode)
         {
            case Keys.Down:
               MoveLine(1);
               break;
            case Keys.Up:
               MoveLine(-1);
               break;
            case Keys.Space:
               this.OK();
               break;
            case Keys.Tab:
               this.Back();
               break;
         }

      }
   }
}
