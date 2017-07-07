﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
      string filter = "";

      public string Filter
      {
         get
         {
            return this.filter;
         }
         set
         {
            this.filter = value;
            this.Line = 0;
         }
      }
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
         this.Reset();

         InitializeComponent();
      }

      public void Reset()
      {
         this.root = new DirectoryInfo(@"C:\Users\soh\Desktop\SHORTCUTTY");
         this.current = this.root;
         this.Filter = "";
         this.Invalidate();
      }

      protected override void OnMouseWheel(MouseEventArgs e)
      {
         base.OnMouseWheel(e);
         MoveLine(e.Delta > 0 ? -1 : 1);

      }
      Font f = new Font("Consolas", 16, FontStyle.Regular, GraphicsUnit.Pixel);
      void PaintLine(string s, Graphics g, int lineNum)
      {
         var offset = 1;
         if (line == lineNum)
         {
            g.FillRectangle(Brushes.White, 0, (lineNum + offset) * f.Height, g.ClipBounds.Width, f.Height);
            g.DrawString(s, this.f, Brushes.Black, 0, (lineNum + offset) * f.Height);
         }
         else
         {
            g.DrawString(s, this.f, Brushes.White, 0, (lineNum+offset) * f.Height);
         }
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
                  var found = !list.Any((ff)=>ff.Name == f.Name) && f.Name.ToUpper().Contains(filter.ToUpper());
                  if (found)
                  {
                     list.Add(f);
                  }
               }
            }
            //list.AddRange(files.Where((f)=>f.Name.ToUpper().Contains(filter.ToUpper())));
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


         g.DrawString(this.Filter, this.f, Brushes.DarkGray, 0, 0 * f.Height);

      }

      private void Form1_KeyPress(object sender, KeyPressEventArgs e)
      {
         if (Char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == ' ')
         {
            this.Filter += e.KeyChar;
            this.Filter = this.Filter.TrimStart();
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
               Process.Start(selected.FullName);
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

      protected override void OnVisibleChanged(EventArgs e)
      {
         if (this.Visible)
         {
            this.Focus();
            this.BringToFront();
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            Cursor.Hide();
         }
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);
         this.Hide();
      }

      private void Form1_Deactivate(object sender, EventArgs e)
      {
         this.Hide();
      }

      private void Form1_MouseClick(object sender, MouseEventArgs e)
      {
         this.OK();
      }
   }
}
