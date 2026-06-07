using System;
using System;
using System.Windows.Forms;
using StudentIS.Forms;
using StudentIS.Services;

namespace StudentIS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DataService.Load();

            Application.Run(new MainForm());
        }
    }
}