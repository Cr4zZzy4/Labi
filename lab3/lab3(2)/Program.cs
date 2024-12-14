using System;
using System.Windows.Forms;

namespace WindowsFormsApp1 // Проверьте, совпадает ли это с пространством имен в Form1.cs
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
