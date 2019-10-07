using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adantino_2
{
    public class Map
    {
        const int fieldRadius = 9;    //Field radius in each direction 

        public bool black { get; set; }
        public int[,] myField { get; set; }
        public List<int[,]> moveList { get; set; }
        public int moveCounter { get; set; }

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
                        int checker = checkNeighbors(y, x, 1, 2, checkPos);

                        if (checker >= 2)
                            checkPos[r + fieldRadius, q + fieldRadius] = 3;
                        else
                            checkPos[r + fieldRadius, q + fieldRadius] = 0;
                    }
                }
            }

            return checkPos;
        }

        public int checkNeighbors(int y, int x, int typeOne, int typeTwo, int[,] checkPos)
        {
            int i = 0;

            if (y + 1 <= 20)
                if (checkPos[y + 1, x] == typeOne || checkPos[y + 1, x] == typeTwo)
                    i++;

            if (y + 1 <= 20 && x - 1 >= 0)
                if (checkPos[y + 1, x - 1] == typeOne || checkPos[y + 1, x - 1] == typeTwo)
                    i++;

            if (x - 1 >= 0)
                if (checkPos[y, x - 1] == typeOne || checkPos[y, x - 1] == typeTwo)
                    i++;

            if (x + 1 >= 0)
                if (checkPos[y, x + 1] == typeOne || checkPos[y, x + 1] == typeTwo)
                    i++;

            if (y - 1 >= 0)
                if (checkPos[y - 1, x] == typeOne || checkPos[y - 1, x] == typeTwo)
                    i++;

            if (x + 1 <= 20 && y - 1 >= 0)
                if (checkPos[y - 1, x + 1] == typeOne || checkPos[y - 1, x + 1] == typeTwo)
                    i++;

            return i;
        }

        public int alphaBeta(Move move, int[,] scoreField, int depth, int alpha, int beta, bool black)
        {
            if(depth == 0)
            {
                if(black)
                    return evalPos(scoreField, true);
                else
                    return evalPos(scoreField, false);
            }

            //Console.WriteLine("checking move: " + (move.r +fieldRadius) + ";" + (move.q + fieldRadius));

            if (black)
            {
                scoreField = checkPosMoves(scoreField);

                List<Move> posAB = getPosMoves(scoreField);

                //Console.WriteLine("Possible moves for AB " + black + " : " + posAB.Count);
                int maxEval = -100000999;
                int eval = 0;

                for (int i = 0; i < posAB.Count; i++)
                {
                    Move bufferMove = posAB.ElementAt(i);

                    //Make a deep copy
                    int[,] bufferField = scoreField.Clone() as int[,];

                    bufferField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 1;

                    eval = alphaBeta(bufferMove, bufferField, depth - 1, alpha, beta, false);

                    //Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) + " ; " + (bufferMove.q + fieldRadius) + " " + eval);

                    maxEval = Math.Max(maxEval, eval);

                    
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                    
                    
                }

                return maxEval;
            }
            else
            {
                scoreField = checkPosMoves(scoreField);

                List<Move> posAB = getPosMoves(scoreField);

                //Console.WriteLine("Possible moves for AB " + black + " : " + posAB.Count);

                int minEval = 100000999;
                int eval = 0;
                for (int i = 0; i < posAB.Count; i++)
                {
                    Move bufferMove = posAB.ElementAt(i);

                    //Make a deep copy
                    int[,] bufferField = scoreField.Clone() as int[,];

                    bufferField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 2;

                    eval = alphaBeta(bufferMove, bufferField, depth - 1, alpha, beta, true);

                    minEval = Math.Min (minEval, eval);
                    
                    beta = Math.Min (beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                    
                }

                return minEval;
            }
        }

        public int evalPos(int[,] evalField, bool black)
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


           

            if ((rbestrow >= 5) && black)
            {
                reward = 1000000;
                Console.WriteLine("preventing red to win");
            }

            if ((bbestrow >= 5) && !black)
            {
                reward = -1000000;
                Console.WriteLine("preventing black to win");
            }


            if ((bbestrow >= 5) && black)
            {
                reward = 200;
            }

            if ((rbestrow >= 5) && !black)
            {
                reward = -200;
            }


            //Console.WriteLine("Bestrow: " + bestrow + " for " + black);

            return reward;
        }

        public int checkWin()
        {
            int win = 0;

            int currentPlayer = 0;
            int bestrowPlayer = 0;

            int row = 0;
            int bestrow = 0;

            //check for diagonal ( / ) 5 in a row 
            for (int s = -(fieldRadius); s <= fieldRadius; s++)
            {
                int q1 = Math.Max(-fieldRadius, -s - fieldRadius);
                int q2 = Math.Min(fieldRadius, -s + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    int r = -(q) - (s);
                    //just check Played fields
                    if (myField[r + fieldRadius, q + fieldRadius] == 1 || myField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        if (myField[r + fieldRadius, q + fieldRadius] == currentPlayer)
                        {
                            row++;
                        }
                        else
                        {
                            row = 1;
                            currentPlayer = myField[r + fieldRadius, q + fieldRadius];
                        }

                        if (row > bestrow)
                        {
                            bestrow = row;
                            bestrowPlayer = currentPlayer;
                        }
                    }
                }
                row = 0;
            }

            //check for diagonal ( \ ) 5 in a row 
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    //just check Played fields
                    if(myField[r + fieldRadius, q + fieldRadius] == 1 || myField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        if (myField[r + fieldRadius, q + fieldRadius] == currentPlayer)
                        {
                            row++;
                        }
                        else 
                        {
                            row = 1;
                            currentPlayer = myField[r + fieldRadius, q + fieldRadius];
                        }

                        if (row > bestrow)
                        {
                            bestrow = row;
                            bestrowPlayer = currentPlayer;
                        }
                    }
                }
                row = 0;
            }

            //check for horizontal ( - ) 5 in a row 
            for (int r = -(fieldRadius); r <= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r - fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    //just check Played fields
                    if (myField[r + fieldRadius, q + fieldRadius] == 1 || myField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        if (myField[r + fieldRadius, q + fieldRadius] == currentPlayer)
                        {
                            row++;
                        }
                        else
                        {
                            row = 1;
                            currentPlayer = myField[r + fieldRadius, q + fieldRadius];
                        }

                        if (row > bestrow)
                        {
                            bestrow = row;
                            bestrowPlayer = currentPlayer;
                        }
                    }
                }
                row = 0;
            }

            if (bestrow >= 5)
            {
                win = bestrowPlayer;
                removeMoves(3);
            }
                
            return win;
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
                    Console.WriteLine("Pos: " + (r + fieldRadius) + " ; " + (q + fieldRadius) + " set to black");
                }
                else
                {
                    myField[r + fieldRadius, q + fieldRadius] = 2;
                    Console.WriteLine("Pos: " + (r + fieldRadius) + " ; " + (q + fieldRadius) + " set to white");
                }

                removeMoves(4);

                black = !black;

                //Check the field for possible moves
                myField = checkPosMoves(myField);

                //Check the field for 5 in row
                win = checkWin();

                //Get all possible moves in an List
                List<Move> posMovesList = getPosMoves(myField);

                Console.WriteLine("Possible moves for " + black + ": " + posMovesList.Count);

                int highScore = 0;
                Move bestMove = new Move(-3, -3);

                for(int i = 0; i< posMovesList.Count; i++)
                {
                    Move bufferMove = posMovesList.ElementAt(i);

                    //Make a deep copy
                    int[,] evalField = myField.Clone() as int[,];

                    if(black)
                        evalField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 1;
                    else
                        evalField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 2;


                    int score = alphaBeta(bufferMove, evalField, 0, -123456, 123456, black);

                    Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) +" ; " + (bufferMove.q + fieldRadius) + " " + score);

                    if(black)
                    {
                        if (score >= highScore)
                        {
                            highScore = score;
                            bestMove = bufferMove;
                        }
                    }
                    else
                    {
                        if (score <= highScore)
                        {
                            highScore = score;
                            bestMove = bufferMove;
                        }
                    }
                }

                if(win == 0)
                    myField[(bestMove.r + fieldRadius), (bestMove.q + fieldRadius)] = 4;

                //Make a deep copy
                int[,] bufferField = myField.Clone() as int[,];

                //add move to moveList
                moveList.Add(bufferField);
                moveCounter++;

                return win;
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
            myField[8, 10] = myField[8, 9] = myField[9, 8] = myField[10, 8] = myField[10, 9] = myField[9, 10] = 3;

            //Make a deep copy
            int[,] bufferField = myField.Clone() as int[,];

            //add start Position to moveList
            moveList.Add(bufferField);
        }

            
    }

}
