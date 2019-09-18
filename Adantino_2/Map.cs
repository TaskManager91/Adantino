using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adantino_2
{
    public class Map
    {
        int fieldRadius = 9;    //Field radius in each direction 
        int fieldSize = 27;     //Field size in px

        //public int[,] myField = new int[20, 20];           //The "field"
        //public List<int[,]> moveList = new List<int[,]>(); //List that stores each move
        //public int moveCounter = 0;    //how many moves have been made

        public bool black { get; set; }
        public int[,] myField { get; set; }
        public List<int[,]> moveList { get; set; }
        public int moveCounter { get; set; }

        public void checkPosMoves()
        {
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (myField[r + fieldRadius, q + fieldRadius] == 1){}
                    else if (myField[r + fieldRadius, q + fieldRadius] == 2){}
                    else if (myField[r + fieldRadius, q + fieldRadius] == 3 || myField[r + fieldRadius, q + fieldRadius] == 0)
                    {
                        int checker = 0;
                        int y = r + fieldRadius;
                        int x = q + fieldRadius;

                        //check if at least two neighbors are set
                        checker = checkNeighbors(x, y, 1, 2);

                        if (checker >= 2)
                            myField[r + fieldRadius, q + fieldRadius] = 3;
                        else
                            myField[r + fieldRadius, q + fieldRadius] = 0;
                    }
                }
            }
        }

        public int checkNeighbors(int x, int y, int typeOne, int typeTwo)
        {
            int i = 0;

            if (y + 1 <= 20)
                if (myField[y + 1, x] == typeOne || myField[y + 1, x] == typeTwo)
                    i++;

            if (y + 1 <= 20 && x - 1 >= 0)
                if (myField[y + 1, x - 1] == typeOne || myField[y + 1, x - 1] == typeTwo)
                    i++;

            if (x - 1 >= 0)
                if (myField[y, x - 1] == typeOne || myField[y, x - 1] == typeTwo)
                    i++;

            if (x + 1 >= 0)
                if (myField[y, x + 1] == typeOne || myField[y, x + 1] == typeTwo)
                    i++;

            if (y - 1 >= 0)
                if (myField[y - 1, x] == typeOne || myField[y - 1, x] == typeTwo)
                    i++;

            if (x + 1 <= 20 && y - 1 >= 0)
                if (myField[y - 1, x + 1] == typeOne || myField[y - 1, x + 1] == typeTwo)
                    i++;

            return i;
        }

        public int checkWin()
        {
            return 0;
        }

        public bool makeMove(int r, int q)
        {
            if (myField[(int)r + fieldRadius, (int)q + fieldRadius] != 3)
            {
                return false;
            }
            else
            {
                if (black)
                {
                    myField[(int)r + fieldRadius, (int)q + fieldRadius] = 1;
                    Console.WriteLine("Pos: " + ((int)q + fieldRadius) + " ; " + ((int)r + fieldRadius) + " set to Black");
                }
                else
                {
                    myField[(int)r + fieldRadius, (int)q + fieldRadius] = 2;
                    Console.WriteLine("Pos: " + ((int)q + fieldRadius) + " ; " + ((int)r + fieldRadius) + " set to Red");
                }
                checkPosMoves();

                //Make a deep copy
                int[,] bufferField = myField.Clone() as int[,];

                //add move to moveList
                moveList.Add(bufferField);
                moveCounter++;

                black = !black;

                return true;
            }
        }

        public void initField()
        {
            myField = new int[20, 20];
            moveList = new List<int[,]>();
            moveCounter = 0;
            black = false;

            //filling the Array with -1
            for (int i = 0; i < 20; i++)
            {
                for (int y = 0; y < 20; y++)
                {
                    myField[y, i] = -1;
                }
            }

            //filling the "playable" field with 0
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    myField[r + fieldRadius, q + fieldRadius] = 0;
                }
            }

            //startPosition
            myField[9, 9] = 1;
            myField[8, 10] = myField[8, 9] = myField[9, 8] = myField[10, 8] = myField[10, 9] = myField[9, 10] = 3;

            //Make a deep copy
            int[,] bufferField = myField.Clone() as int[,];

            //add start Position to moveList
            moveList.Add(bufferField);
        }

            
    }

}
