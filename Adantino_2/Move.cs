using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adantino
{
    public class Move
    {
        public int r { get; set; }

        public int q { get; set; }

        public int score { get; set; }

        //simple class to store a move
        public Move(int bufferR, int bufferQ)
        {
            r = bufferR;
            q = bufferQ;
        }
    }
}
