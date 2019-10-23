using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace Adantino_2
{
    static class Program
    {
        /// <summary>
        /// Adantino The Game
        /// </summary>
        [STAThread]
        static void Main()
        {
            Map myMap = new Map();
            myMap.initField();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(myMap));
        }
    }
}
