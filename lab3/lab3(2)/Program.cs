using System;
using System.Windows.Forms;

namespace WindowsFormsApp1 // ���������, ��������� �� ��� � ������������� ���� � Form1.cs
{
    static class Program
    {
        /// <summary>
        /// ������� ����� ����� ��� ����������.
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
