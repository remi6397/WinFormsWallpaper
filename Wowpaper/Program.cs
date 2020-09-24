/* Any copyright is dedicated to the Public Domain.
 * https://creativecommons.org/publicdomain/zero/1.0/ */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Wowpaper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string startUrl;
            if (args.Length >= 1)
                startUrl = args[0];
            else
                startUrl = Interaction.InputBox("Enter start URL", DefaultResponse: "https://en.wikipedia.org/wiki/Rick_Astley");

            Application.Run(new RootForm(startUrl));
        }
    }
}
