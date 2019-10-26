using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace Adantino
{
    static class Program
    {
        /// 
        /// Adantino implementation with AlphaBeta
        /// By Christoph Bensch
        [STAThread]
        static void Main()
        {
            //create Map object
            Map myMap = new Map();

            //Init field
            myMap.initField();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //create GameForm & pass map object to it
            Application.Run(new GameForm(myMap));
        }
    }
}
