using System;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Здесь создается экземпляр Form1
        }
    }
}
