using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.Diagnostics;

namespace VsBoleto
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            string x = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(x).Length > 1)
            {
                MessageBox.Show("Aplicação já está em execução!", "Erro");
                Application.Exit();
            }
            else
            {
                Application.Run(new Sistema.Principal());
            }
                

            
        }
    }
}
