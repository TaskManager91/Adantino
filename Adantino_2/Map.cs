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
        const int fieldRadius = 9;          //Field radius in each direction 
        public bool black { get; set; }     //if true, its blacks turn, otherwise reds turn
        public bool blackAI { get; set; }   //is black AI activated?
        public bool redAI { get; set; }     //is red AI activated?
        public bool abReady { get; set; }
        public int aiDepth { get; set; }    //current Alpha-Beta depth
        public int[,] myField { get; set; } //The Map is stored here   
        public List<int[,]> moveList { get; set; }  //Stores a list of Moves, important for the undo function
        public int moveCounter { get; set; }   

        //init the Game field
        public void initField()
        {
            aiDepth = 0;
            myField = new int[20, 20];
            moveList = new List<int[,]>();
            moveCounter = 0;
            black = false;

            blackAI = true;
            redAI = true;

            //filling the whole array with -1
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

            //start position
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

                //remove last recommendation
                removeMoves(4);

                //Check the field for possible moves
                myField = checkPosMoves(myField);

                CheckWin check = new CheckWin(this);

                //Check for winner
                win = check.checkWin(myField);

                //Next players turn!
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

        //checks neighbors for a Type returns count of it
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

        //returns a map filled with possible moves = 3
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

        //returns a list<moves> of possible moves
        public List<Move> getPosMoves(int[,] checkList)
        {
            List<Move> posMovesList = new List<Move>();
            
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    //if the field is playable (=3) add to List
                    if (checkList[r + fieldRadius, q + fieldRadius] == 3)
                    {
                        Move move = new Move(r, q);
                        posMovesList.Add(move);
                    }
                }
            }

            return posMovesList;
        }

        //returns a list<moves> of all fields that are NOT the enemy
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

        //returns a list<moves> of moves, that fit the currentPlayer
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

        //removes moves of the type provided
        public void removeMoves(int what)
        {
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    //if its the type remove it
                    if (myField[r + fieldRadius, q + fieldRadius] == what)
                    {
                        myField[r + fieldRadius, q + fieldRadius] = 0;
                    }
                }
            }
        }

        //swap moves from -> to
        public void swapMoves(int from, int to)
        {
            //run through the field
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
