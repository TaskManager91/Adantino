using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Adantino
{
    public class Map
    {
        const int fieldRadius = 9;    //Field radius in each direction 
        public bool black { get; set; }
        public bool blackAI { get; set; }
        public bool redAI { get; set; }
        public bool abReady { get; set; }
        public int aiDepth { get; set; }
        public int[,] myField { get; set; }
        public List<int[,]> moveList { get; set; }
        public int moveCounter { get; set; }

        public void initField()
        {
            aiDepth = 0;
            myField = new int[20, 20];
            moveList = new List<int[,]>();
            moveCounter = 0;
            black = false;

            blackAI = true;
            redAI = true;

            //filling the Array with -1
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    myField[j, i] = -1;
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
            myField[8, 10] = myField[8, 9] = myField[9, 8] = myField[10, 8] = myField[9, 10] = 3;
            myField[10, 9] = 4;

            //Make a deep copy
            int[,] bufferField = myField.Clone() as int[,];

            //add start Position to moveList
            moveList.Add(bufferField);
        }

        public int makeMove(int r, int q)
        {
            int win = -1;

            if (myField[r + fieldRadius, q + fieldRadius] != 3 && myField[r + fieldRadius, q + fieldRadius] != 4)
            {
                //Invalid move! -> not playable
                return win;
            }
            else
            {
                if (black)
                {
                    myField[r + fieldRadius, q + fieldRadius] = 1;
                    Console.WriteLine("SET POS: " + (r + fieldRadius) + " ; " + (q + fieldRadius) + " set to black");
                }
                else
                {
                    myField[r + fieldRadius, q + fieldRadius] = 2;
                    Console.WriteLine("SET POS: " + (r + fieldRadius) + " ; " + (q + fieldRadius) + " set to red");
                }

                //remove last recommondation
                removeMoves(4);

                //Check the field for possible moves
                myField = checkPosMoves(myField);

                CheckWin check = new CheckWin(this);

                //Check for winner
                win = check.checkWin(myField);

                //Next players turn!!
                black = !black;

                if (win == 1 || win == 2)
                    removeMoves(3);

                //Make a deep copy
                int[,] bufferField = myField.Clone() as int[,];

                //add move to moveList
                moveList.Add(bufferField);
                moveCounter++;

                return win;
            }
        }

        public int checkNeighbors(int r, int q, int type, int[,] checkPos)
        {
            int i = 0;

            if (r + 1 <= 20)
                if (checkPos[r + 1, q] == type)
                    i++;

            if (r + 1 <= 20 && q - 1 >= 0)
                if (checkPos[r + 1, q - 1] == type)
                    i++;

            if (q - 1 >= 0)
                if (checkPos[r, q - 1] == type)
                    i++;

            if (q + 1 <= 20)
                if (checkPos[r, q + 1] == type)
                    i++;

            if (r - 1 >= 0)
                if (checkPos[r - 1, q] == type)
                    i++;

            if (q + 1 <= 20 && r - 1 >= 0)
                if (checkPos[r - 1, q + 1] == type)
                    i++;

            return i;
        }

        public int[,] checkPosMoves(int[,] checkPos)
        {
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (checkPos[r + fieldRadius, q + fieldRadius] == 3 || checkPos[r + fieldRadius, q + fieldRadius] == 0 || checkPos[r + fieldRadius, q + fieldRadius] == 4)
                    {
                        int y = r + fieldRadius;
                        int x = q + fieldRadius;

                        //check if at least two neighbors are set
                        int checker = 0;
                        checker += checkNeighbors(y, x, 1, checkPos);
                        checker += checkNeighbors(y, x, 2, checkPos);

                        if (checker >= 2)
                            checkPos[r + fieldRadius, q + fieldRadius] = 3;
                        else
                            checkPos[r + fieldRadius, q + fieldRadius] = 0;
                    }
                }
            }

            return checkPos;
        }

        public List<Move> getPosMoves(int[,] checkList)
        {
            List<Move> posMovesList = new List<Move>();

            //remove all possible moves / make unplayable
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (checkList[r + fieldRadius, q + fieldRadius] == 3)
                    {
                        Move move = new Move(r, q);
                        posMovesList.Add(move);
                    }
                }
            }

            return posMovesList;
        }

        public List<Move> getMyEnemyNeighbors(int rBuffer, int qBuffer, int[,] checkField, int enemy)
        {
            List<Move> myNeighbors = new List<Move>();

            int y = rBuffer;
            int x = qBuffer;

            int checkType = enemy;

            if (y + 1 <= 20)
                if (checkField[y + 1, x] != checkType)
                {
                    Move move = new Move(y + 1, x);
                    myNeighbors.Add(move);
                }

            if (y + 1 <= 20 && x - 1 >= 0)
                if (checkField[y + 1, x - 1] != checkType)
                {
                    Move move = new Move(y + 1, x - 1);
                    myNeighbors.Add(move);
                }

            if (x - 1 >= 0)
                if (checkField[y, x - 1] != checkType)
                {
                    Move move = new Move(y, x - 1);
                    myNeighbors.Add(move);
                }

            if (x + 1 <= 20)
                if (checkField[y, x + 1] != checkType)
                {
                    Move move = new Move(y, x + 1);
                    myNeighbors.Add(move);
                }

            if (y - 1 >= 0)
                if (checkField[y - 1, x] != checkType)
                {
                    Move move = new Move(y - 1, x);
                    myNeighbors.Add(move);
                }

            if (x + 1 <= 20 && y - 1 >= 0)
                if (checkField[y - 1, x + 1] != checkType)
                {
                    Move move = new Move(y - 1, x + 1);
                    myNeighbors.Add(move);
                }

            return myNeighbors;
        }

        public List<Move> getMyNeighbors(int rBuffer, int qBuffer, int[,] checkField, int currentPlayer)
        {
            List<Move> myNeighbors = new List<Move>();

            int y = rBuffer;
            int x = qBuffer;

            int myType = currentPlayer;

            if (y + 1 <= 20)
                if (checkField[y + 1, x] == myType)
                {
                    Move move = new Move(y + 1, x);
                    myNeighbors.Add(move);
                }
                    
            if (y + 1 <= 20 && x - 1 >= 0)
                if (checkField[y + 1, x - 1] == myType)
                {
                    Move move = new Move(y + 1, x -1);
                    myNeighbors.Add(move);
                }

            if (x - 1 >= 0)
                if (checkField[y, x - 1] == myType)
                {
                    Move move = new Move(y, x - 1);
                    myNeighbors.Add(move);
                }

            if (x + 1 <= 20)
                if (checkField[y, x + 1] == myType)
                {
                    Move move = new Move(y, x+1);
                    myNeighbors.Add(move);
                }

            if (y - 1 >= 0)
                if (checkField[y - 1, x] == myType)
                {
                    Move move = new Move(y - 1, x);
                    myNeighbors.Add(move);
                }

            if (x + 1 <= 20 && y - 1 >= 0)
                if (checkField[y - 1, x + 1] == myType)
                {
                    Move move = new Move(y - 1, x + 1);
                    myNeighbors.Add(move);
                }

            return myNeighbors;
        }

        public void removeMoves(int what)
        {
            //remove all possible moves / make unplayable
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (myField[r + fieldRadius, q + fieldRadius] == what)
                    {
                        myField[r + fieldRadius, q + fieldRadius] = 0;
                    }
                }
            }
        }

        public void swapMoves(int from, int to)
        {
            //remove all possible moves / make unplayable
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (myField[r + fieldRadius, q + fieldRadius] == from)
                    {
                        myField[r + fieldRadius, q + fieldRadius] = to;
                    }
                }
            }
        }

        
    }

}
