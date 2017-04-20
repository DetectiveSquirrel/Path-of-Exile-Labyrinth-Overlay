using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication6
{
    public partial class Form1 : Form
    {
        public class MyClassName
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int X { get; internal set; }
            public int Y { get; internal set; }
            public int Width { get; internal set; }
            public int Height { get; internal set; }
        }

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.Image = (Bitmap)Image.FromFile(@"Content\lab.png", true);
            if (GetPathOfExileProcess() != null)
            {
                InitTimer();
            }
            else
            {
                this.Close();
            }

        }

        private Timer timer1;
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 250; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetPathOfExileProcess() != null)
            {
                Rect NotepadRect = new Rect();
                IntPtr ptr = GetPathOfExileProcess().MainWindowHandle;
                GetWindowRect(ptr, ref NotepadRect);

                if (GetForegroundWindow() == ptr)
                {
                    NotepadRect.X = NotepadRect.Left;
                    NotepadRect.Y = NotepadRect.Top;
                    NotepadRect.Width = NotepadRect.Right - NotepadRect.Left + 1;
                    NotepadRect.Height = NotepadRect.Bottom - NotepadRect.Top + 1;

                    if (Properties.Settings.Default.Top)
                    {
                        this.Top = NotepadRect.Top;
                        this.Left = (NotepadRect.Width - this.Width) / 2 + NotepadRect.Left;
                    }
                    if (Properties.Settings.Default.Bottom)
                    {
                        this.Top = NotepadRect.Bottom - this.Height;
                        this.Left = (NotepadRect.Width - this.Width) / 2 + NotepadRect.Left;
                    }
                    if (Properties.Settings.Default.Left)
                    {
                        this.Top = (NotepadRect.Height - this.Height) / 2 + NotepadRect.Top;
                        this.Left = NotepadRect.Left;
                    }
                    if (Properties.Settings.Default.Right)
                    {
                        this.Top = (NotepadRect.Height - this.Height) / 2 + NotepadRect.Top;
                        this.Left = NotepadRect.Right - this.Width;
                    }
                    if (Properties.Settings.Default.TopLeft)
                    {
                        this.Top = NotepadRect.Top;
                        this.Left = NotepadRect.Left;
                    }
                    if (Properties.Settings.Default.TopRight)
                    {
                        this.Top = NotepadRect.Top;
                        this.Left = NotepadRect.Right - this.Width;
                    }
                    if (Properties.Settings.Default.BottomLeft)
                    {
                        this.Top = NotepadRect.Bottom - this.Height;
                        this.Left = NotepadRect.Left;
                    }
                    if (Properties.Settings.Default.BottomRight)
                    {
                        this.Top = NotepadRect.Bottom - this.Height;
                        this.Left = NotepadRect.Right - this.Width;
                    }
                }
                else
                {
                    // Throw that shit somewhere non visable because minimizing back and forth just caused fat ass hell issues that i didnt want to deal with
                    this.Left = 999999;
                    this.Top = 999999;
                }
            }
            else
            {
                this.Close();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Set the form click-through
                cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */;
                return cp;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Pretty useless now that im drawing it on path of exile window only when main focus
        // but its still a toggle if you want it on and off quite often
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        // Grab the poe processes
        public Process GetPathOfExileProcess()
        {
            Process[] processes = Process.GetProcesses(".");
            // Process names to look for, add if yours is missing
            string[] PathofExileExecutables = { "PathOfExile", "PathOfExile_x64", "PathOfExileSteam", "PathOfExile_x64Steam" };

            for (var i = 0; i < processes.Length; i++)
            {
                IntPtr windowHandle = processes[i].MainWindowHandle;

                for (var j = 0; j < PathofExileExecutables.Length; j++)
                {
                    if (processes[i].ProcessName == PathofExileExecutables[j])
                    {
                        return processes[i];
                    }
                }
            }

            return null;
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = true;
            Properties.Settings.Default.Bottom = false;
            Properties.Settings.Default.Left = false;
            Properties.Settings.Default.Right = false;
            Properties.Settings.Default.TopRight = false;
            Properties.Settings.Default.TopLeft = false;
            Properties.Settings.Default.BottomLeft = false;
            Properties.Settings.Default.BottomRight = false;
            Properties.Settings.Default.Save();
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = false;
            Properties.Settings.Default.Bottom = true;
            Properties.Settings.Default.Left = false;
            Properties.Settings.Default.Right = false;
            Properties.Settings.Default.TopRight = false;
            Properties.Settings.Default.TopLeft = false;
            Properties.Settings.Default.BottomLeft = false;
            Properties.Settings.Default.BottomRight = false;
            Properties.Settings.Default.Save();
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = false;
            Properties.Settings.Default.Bottom = false;
            Properties.Settings.Default.Left = true;
            Properties.Settings.Default.Right = false;
            Properties.Settings.Default.TopRight = false;
            Properties.Settings.Default.TopLeft = false;
            Properties.Settings.Default.BottomLeft = false;
            Properties.Settings.Default.BottomRight = false;
            Properties.Settings.Default.Save();
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = false;
            Properties.Settings.Default.Bottom = false;
            Properties.Settings.Default.Left = false;
            Properties.Settings.Default.Right = true;
            Properties.Settings.Default.TopRight = false;
            Properties.Settings.Default.TopLeft = false;
            Properties.Settings.Default.BottomLeft = false;
            Properties.Settings.Default.BottomRight = false;
            Properties.Settings.Default.Save();
        }

        private void topLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = false;
            Properties.Settings.Default.Bottom = false;
            Properties.Settings.Default.Left = false;
            Properties.Settings.Default.Right = false;
            Properties.Settings.Default.TopRight = false;
            Properties.Settings.Default.TopLeft = true;
            Properties.Settings.Default.BottomLeft = false;
            Properties.Settings.Default.BottomRight = false;
            Properties.Settings.Default.Save();
        }

        private void topRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = false;
            Properties.Settings.Default.Bottom = false;
            Properties.Settings.Default.Left = false;
            Properties.Settings.Default.Right = false;
            Properties.Settings.Default.TopRight = true;
            Properties.Settings.Default.TopLeft = false;
            Properties.Settings.Default.BottomLeft = false;
            Properties.Settings.Default.BottomRight = false;
            Properties.Settings.Default.Save();
        }

        private void bottomLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = false;
            Properties.Settings.Default.Bottom = false;
            Properties.Settings.Default.Left = false;
            Properties.Settings.Default.Right = false;
            Properties.Settings.Default.TopRight = false;
            Properties.Settings.Default.TopLeft = false;
            Properties.Settings.Default.BottomLeft = true;
            Properties.Settings.Default.BottomRight = false;
            Properties.Settings.Default.Save();
        }

        private void bottomRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Top = false;
            Properties.Settings.Default.Bottom = false;
            Properties.Settings.Default.Left = false;
            Properties.Settings.Default.Right = false;
            Properties.Settings.Default.TopRight = false;
            Properties.Settings.Default.TopLeft = false;
            Properties.Settings.Default.BottomLeft = false;
            Properties.Settings.Default.BottomRight = true;
            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
