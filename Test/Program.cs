using ActiveDirectoryAccess;
using CredLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Test
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {

            CredLogin.CredLogin credl = new CredLogin.CredLogin("test", "BSZ.local");
            if (credl.Login(null,"Test Programm"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
