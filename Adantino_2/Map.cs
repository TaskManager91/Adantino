using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Adantino_2
{
    public class Map
    {
        const int fieldRadius = 9;    //Field radius in each direction 

        public bool black { get; set; }

        public bool aiOne { get; set; }
        public bool aiTwo { get; set; }

        public int aiDepth { get; set; }

        public int aiTime { get; set; }

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
            aiOne = true;
            aiTwo = true;

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

        public int checkNeighbors(int y, int x, int type, int[,] checkPos)
        {
            int i = 0;

            if (y + 1 <= 20)
                if (checkPos[y + 1, x] == type)
                    i++;

            if (y + 1 <= 20 && x - 1 >= 0)
                if (checkPos[y + 1, x - 1] == type)
                    i++;

            if (x - 1 >= 0)
                if (checkPos[y, x - 1] == type)
                    i++;

            if (x + 1 <= 20)
                if (checkPos[y, x + 1] == type)
                    i++;

            if (y - 1 >= 0)
                if (checkPos[y - 1, x] == type)
                    i++;

            if (x + 1 <= 20 && y - 1 >= 0)
                if (checkPos[y - 1, x + 1] == type)
                    i++;

            return i;
        }

        public int alphaBeta(Move move, int[,] scoreField, int depth, int alpha, int beta, bool black)
        {
            int win = checkWin(scoreField);

            if (win == 1)
                return 2000;

            if (win == 2)
               return -2000;

            
            if (depth == 0)
                return evalPos(scoreField);
                

            //return evalPos(scoreField);

            //Console.WriteLine("checking move: " + (move.r +fieldRadius) + ";" + (move.q + fieldRadius));

            if (black)
            {
                scoreField = checkPosMoves(scoreField);

                List<Move> posAB = getPosMoves(scoreField);
                
                int maxEval = -9999;
                int eval = 0;

                for (int i = 0; i < posAB.Count; i++)
                {
                    Move bufferMove = new Move(0, 0);
                    bufferMove = posAB.ElementAt(i);

                    //Make a deep copy
                    int[,] bufferField = new int[20, 20];
                    bufferField = scoreField.Clone() as int[,];

                    bufferField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 1;

                    eval = alphaBeta(bufferMove, bufferField, depth - 1, alpha, beta, false);

                    //Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) + " ; " + (bufferMove.q + fieldRadius) + " " + eval);

                    maxEval = Math.Max(maxEval, eval);

                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }

                return maxEval;
            }
            else
            {
                scoreField = checkPosMoves(scoreField);

                List<Move> posAB = getPosMoves(scoreField);
               
                int minEval = 9999;
                int eval = 0;
                for (int i = 0; i < posAB.Count; i++)
                {
                    Move bufferMove = new Move(0, 0);
                    bufferMove = posAB.ElementAt(i);

                    //Make a deep copy
                    int[,] bufferField = new int[20, 20];
                    bufferField = scoreField.Clone() as int[,];

                    bufferField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 2;

                    eval = alphaBeta(bufferMove, bufferField, depth - 1, alpha, beta, true);

                    minEval = Math.Min (minEval, eval);
                    beta = Math.Min (beta, eval);
                    if (beta <= alpha)
                        break;
                              
                }
                return minEval;
            }
        }

        public int evalPos(int[,] evalField)
        {
            int reward = 0;

            int brow = 0;
            int rrow = 0;
            int bbestrow = 0;
            int rbestrow = 0;

            //check for diagonal ( / ) row 
            for (int s = -(fieldRadius); s <= fieldRadius; s++)
            {
                int q1 = Math.Max(-fieldRadius, -s - fieldRadius);
                int q2 = Math.Min(fieldRadius, -s + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    int r = -(q) - (s);

                    if (evalField[r + fieldRadius, q + fieldRadius] == 1)
                        brow++;

                    if (evalField[r + fieldRadius, q + fieldRadius] == 2)
                        rrow++;

                    if (evalField[r + fieldRadius, q + fieldRadius] != 1)
                        brow = 0;

                    if (evalField[r + fieldRadius, q + fieldRadius] != 2)
                        rrow = 0;

                    if (brow > bbestrow)
                        bbestrow = brow;

                    if (rrow > rbestrow)
                        rbestrow = rrow;
                }
                rrow = 0;
                brow = 0;
            }

            /*
            if (rbestrow > bbestrow)
                reward += -100;
            else if (rbestrow < bbestrow)
                reward += 100;
                */

            //check for diagonal ( \ ) row 
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (evalField[r + fieldRadius, q + fieldRadius] == 1)
                        brow++;


                    if (evalField[r + fieldRadius, q + fieldRadius] == 2)
                        rrow++;

                    if (evalField[r + fieldRadius, q + fieldRadius] != 1)
                        brow = 0;

                    if (evalField[r + fieldRadius, q + fieldRadius] != 2)
                        rrow = 0;

                    if (brow > bbestrow)
                        bbestrow = brow;

                    if (rrow > rbestrow)
                        rbestrow = rrow;
                }
                rrow = 0;
                brow = 0;
            }

            /*
            if (rbestrow > bbestrow)
                reward += -100;
            else if (rbestrow < bbestrow)
                reward += 100;
                */

            //check for horizontal ( - ) row 
            for (int r = -(fieldRadius); r <= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r - fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    if (evalField[r + fieldRadius, q + fieldRadius] == 1)
                        brow++;

                    if (evalField[r + fieldRadius, q + fieldRadius] == 2)
                        rrow++;

                    if (evalField[r + fieldRadius, q + fieldRadius] != 1)
                        brow = 0;

                    if (evalField[r + fieldRadius, q + fieldRadius] != 2)
                        rrow = 0;

                    if (brow > bbestrow)
                        bbestrow = brow;

                    if (rrow > rbestrow)
                        rbestrow = rrow;
                }
                rrow = 0;
                brow = 0;
            }

            if (rbestrow > bbestrow)
                reward += -100;
            else if (rbestrow < bbestrow)
                reward += 100;

            //Console.WriteLine("Bestrow: " + bestrow + " for " + black);

            return reward;
        }

        public int checkWin(int[,] checkField)
        {
            int win = 0;

            int bRow = 0;
            int rRow = 0;

            int rBestRow = 0;
            int bBestRow = 0;

            //check for diagonal ( / ) 5 in a row 
            for (int s = -(fieldRadius); s <= fieldRadius; s++)
            {
                int q1 = Math.Max(-fieldRadius, -s - fieldRadius);
                int q2 = Math.Min(fieldRadius, -s + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    int r = -(q) - (s);

                    if (checkField[r + fieldRadius, q + fieldRadius] == 1)
                        bRow++;

                    if (checkField[r + fieldRadius, q + fieldRadius] == 2)
                        rRow++;

                    if (checkField[r + fieldRadius, q + fieldRadius] != 1)
                        bRow = 0;

                    if (checkField[r + fieldRadius, q + fieldRadius] != 2)
                        rRow = 0;

                    if (bRow > bBestRow)
                        bBestRow = bRow;

                    if (rRow > rBestRow)
                        rBestRow = rRow;
                }
                bRow = 0;
                rRow = 0;
            }

            if (bBestRow >= 5)
                return 1;

            if (rBestRow >= 5)
                return 2;

            //check for diagonal ( \ ) 5 in a row 
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                   if (checkField[r + fieldRadius, q + fieldRadius] == 1)
                        bRow++;

                    if (checkField[r + fieldRadius, q + fieldRadius] == 2)
                        rRow++;

                    if (checkField[r + fieldRadius, q + fieldRadius] != 1)
                        bRow = 0;

                    if (checkField[r + fieldRadius, q + fieldRadius] != 2)
                        rRow = 0;

                    if (bRow > bBestRow)
                        bBestRow = bRow;

                    if (rRow > rBestRow)
                        rBestRow = rRow;
                }
                bRow = 0;
                rRow = 0;
            }

            if (bBestRow >= 5)
                return 1;

            if (rBestRow >= 5)
                return 2;

            //check for horizontal ( - ) 5 in a row 
            for (int r = -(fieldRadius); r <= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r - fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    if (checkField[r + fieldRadius, q + fieldRadius] == 1)
                        bRow++;

                    if (checkField[r + fieldRadius, q + fieldRadius] == 2)
                        rRow++;

                    if (checkField[r + fieldRadius, q + fieldRadius] != 1)
                        bRow = 0;

                    if (checkField[r + fieldRadius, q + fieldRadius] != 2)
                        rRow = 0;

                    if (bRow > bBestRow)
                        bBestRow = bRow;

                    if (rRow > rBestRow)
                        rBestRow = rRow;
                }
                bRow = 0;
                rRow = 0;
            }

            if (bBestRow >= 5)
                return 1;

            if (rBestRow >= 5)
                return 2;

            int[,] bufferField = new int[20, 20];
            bufferField = checkField.Clone() as int[,];
            win = checkPrisoners(bufferField);
                
            return win;
        }

        public int checkPrisoners(int[,] checkField)
        {
            int win = 0;

            for (int r = -(fieldRadius); r <= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r - fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    if(checkField[r+fieldRadius, q+fieldRadius] == 1 || checkField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        int currentPlayer = checkField[r + fieldRadius, q + fieldRadius];
                        //check if at least one neighbor is free
                        int checker = 0;
                        checker += checkNeighbors(r + fieldRadius, q + fieldRadius, 0, checkField);
                        
                        if (checker == 0)
                            checker += checkNeighbors(r + fieldRadius, q + fieldRadius, 3, checkField);

                        if (checker == 0)
                            checker += checkNeighbors(r + fieldRadius, q + fieldRadius, 4, checkField);

                        if (checker == 0 && currentPlayer == 1)
                            checker += checkNeighbors(r + fieldRadius, q + fieldRadius, 5, checkField);

                        if (checker == 0 && currentPlayer == 2)
                            checker += checkNeighbors(r + fieldRadius, q + fieldRadius, 6, checkField);

                        //Console.WriteLine("FREE Fields: " + (r + fieldRadius) + " ; " + (q + fieldRadius) + " " + checker);

                        //Jaay, free field => set to 5
                        if (checker > 0)
                        {
                            if (currentPlayer == 1)
                                checkField[r + fieldRadius, q + fieldRadius] = 5; //Black free field
                            else
                                checkField[r + fieldRadius, q + fieldRadius] = 6; //Red free field
                        }                            
                        else
                        {
                            checkField[r + fieldRadius, q + fieldRadius] = 99; //Field checked -> ignore in deeper steps to prevent stack overflow

                            checker = checkPrisonersHelper(r + fieldRadius, q + fieldRadius, checkField, currentPlayer);
                        }
                            

                        if (checker == 0)
                        {
                            //Console.WriteLine("Fields CAPTURED: " + (r + fieldRadius) + " ; " + (q + fieldRadius) + " " + checker);
                            if (currentPlayer == 1)
                                return 2;
                            else
                                return 1;
                        } 
                        else
                        {
                            if (currentPlayer == 1)
                                checkField[r + fieldRadius, q + fieldRadius] = 5;
                            else
                                checkField[r + fieldRadius, q + fieldRadius] = 6;
                        }
                          
                    }
                }
            }
            return win;
        }

        public int checkPrisonersHelper(int r, int q, int[,] checkField, int currentPlayer)
        {
            //Console.WriteLine("need to dig deeper!");
            //int currentPlayer = checkField[r, q];

            int win = 0;

            List<Move> myNeighbors = new List<Move>();

            myNeighbors = getMyNeighbors(r, q, checkField, currentPlayer);

            //Console.WriteLine("R,q: " + r + " " + q + " checking count of neighbors: " + myNeighbors.Count);

            for(int i=0; i<myNeighbors.Count; i++)
            {
                
                Move move = myNeighbors.ElementAt(i);
                //Console.WriteLine("Checking Neighbor (r,q): " + move.r + " " + move.q);
                //check if at least one neighbor of Neighbor is free
                int checker = 0;
                checker += checkNeighbors(move.r, move.q, 0, checkField);

                if (checker == 0)
                    checker += checkNeighbors(move.r, move.q, 3, checkField);

                if (checker == 0)
                    checker += checkNeighbors(move.r, move.q, 4, checkField);

                if (checker == 0 && currentPlayer == 1)
                    checker += checkNeighbors(r, q, 5, checkField);

                if (checker == 0 && currentPlayer == 2)
                    checker += checkNeighbors(r, q, 6, checkField);

                //Jaay, free field => set to 5 for black or 6 for Red (checked and free) else dig deeper
                if (checker > 0)
                {
                    return 1;
                }
                else
                {
                    //Console.WriteLine("Neighbor (r,q): " + move.r + " " + move.q + " is also trapped going even deeper");
                    checkField[r, q] = 99; //Field checked -> ignore in deeper steps to prevent stack overflow
                    checker = checkPrisonersHelper(move.r, move.q, checkField, currentPlayer);
                }

                if (checker > 0)
                    return 1;
                else
                    return 0;       
            }

            return win;
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
                        Move move = new Move(r,q);
                        posMovesList.Add(move);
                    }
                }
            }

            return posMovesList;
        }

        public int makeMove(int r, int q)
        {
            int win = -1;

            if (myField[r + fieldRadius, q + fieldRadius] == 0 || myField[r + fieldRadius, q + fieldRadius] == 1 || myField[r + fieldRadius, q + fieldRadius] == 2)
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

                //Check for winner
                win = checkWin(myField);

                //Next players turn!!
                black = !black;

                if (win == 1 || win == 2)
                {
                    removeMoves(3);
                }  
                else 
                {
                    aiDepth = 6;

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // START AlphaBeta
                    alphaBetaStart();

                    stopwatch.Stop();
                    TimeSpan stopwatchElapsed = stopwatch.Elapsed;
                    aiTime = Convert.ToInt32(stopwatchElapsed.TotalMilliseconds);

                }
                    
                    

                //Make a deep copy
                int[,] bufferField = myField.Clone() as int[,];

                //add move to moveList
                moveList.Add(bufferField);
                moveCounter++;

                return win;
            }
        }

        public void alphaBetaStart()
        {
            //remove last recommondation
            removeMoves(4);

            //Check the field for possible moves
            myField = checkPosMoves(myField);

            //Get all possible moves in an List
            List<Move> posMovesList = getPosMoves(myField);

            if (black)
                Console.WriteLine("Possible moves for black: " + posMovesList.Count);
            else
                Console.WriteLine("Possible moves for red: " + posMovesList.Count);

            int highScore = 0;

            if (black)
                highScore = -1500;
            else
                highScore = 1500;

            Move bestMove = new Move(0, 0);

            for (int i = 0; i < posMovesList.Count; i++)
            {
                Move bufferMove = new Move(0, 0);
                bufferMove = posMovesList.ElementAt(i);

                //Make a deep copy
                int[,] evalField = new int[20, 20];
                evalField = myField.Clone() as int[,];

                if (black)
                    evalField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 1;
                else
                    evalField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 2;

                //Move Played so other player
                bool bBuffer = !black;

                int score = 0;
                score = alphaBeta(bufferMove, evalField, aiDepth, -9999, 9999, bBuffer);

                if (black)
                {
                    if (score >= highScore)
                    {
                        //Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) + " ; " + (bufferMove.q + fieldRadius) + " " + score);
                        highScore = score;
                        bestMove = bufferMove;
                    }

                }
                else
                {
                    if (score <= highScore)
                    {
                        //Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) + " ; " + (bufferMove.q + fieldRadius) + " " + score);
                        highScore = score;
                        bestMove = bufferMove;
                    }
                }
            }

            if (black && highScore == -1500)
                highScore = -1500; // Enemy won -> no suggestion
            else if (!black && highScore == 1500)
                highScore = 1500; // Enemy won -> no suggestion
            else
                myField[(bestMove.r + fieldRadius), (bestMove.q + fieldRadius)] = 4;
        }


    }

}
