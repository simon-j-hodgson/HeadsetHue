using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;

namespace HeadsetHue
{
    static class Program
    {
        static Form1 form1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.ApplicationExit += Application_ApplicationExit;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();
            Application.Run(form1);
        }

        private static async void Application_ApplicationExit(object sender, EventArgs e)
        {
            //await Form1.LightOff();
        }
    }
}
