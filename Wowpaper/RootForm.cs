/* Any copyright is dedicated to the Public Domain.
 * https://creativecommons.org/publicdomain/zero/1.0/ */
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wowpaper
{
    public partial class RootForm : Form
    {
        private ChromiumWebBrowser browser;

        public RootForm(string startUrl)
        {
            InitializeComponent();

            browser = new ChromiumWebBrowser(startUrl);
            browser.Dock = DockStyle.Fill;
            Controls.Add(browser);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SetAsWallpaper();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}