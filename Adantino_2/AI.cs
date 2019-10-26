using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adantino
{
    class AI
    {
        private Map map;
        private CheckWin check;
        private const int fieldRadius = 9;
        public AI(Map bufferMap)
        {
            map = bufferMap;
            check = new CheckWin(map);
        }

        public void alphaBetaStart()
        {
            //Depth initially to 2
            map.aiDepth = 2;
            map.abReady = false;

            //remove last recommendation
            map.removeMoves(4);

            //Check the field for possible moves
            map.myField = map.checkPosMoves(map.myField);

            //Get all possible moves in an List
            List<Move> posMovesList = map.getPosMoves(map.myField);

            if (map.black)
                Console.WriteLine("Possible moves for black: " + posMovesList.Count);
            else
                Console.WriteLine("Possible moves for red: " + posMovesList.Count);

            int highScore = 0;

            if (map.black)
                highScore = -1500;
            else
                highScore = 1500;

            Move bestMove = new Move(0, 0);

            //just necessary for the kill thread
            while(!map.abReady)
            {
                for (int i = 0; i < posMovesList.Count; i++)
                {
                    Move bufferMove = new Move(0, 0);
                    bufferMove = posMovesList.ElementAt(i);

                    //Make a deep copy
                    int[,] evalField = new int[20, 20];
                    evalField = map.myField.Clone() as int[,];

                    if (map.black)
                        evalField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 1;
                    else
                        evalField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 2;

                    //Move Played so other player
                    bool bBuffer = !map.black;

                    int score = 0;

                    //perform AlphaBeta-Search for the specific move
                    score = alphaBeta(bufferMove, evalField, map.aiDepth, -9999, 9999, bBuffer);

                    //save the score, for moveordering
                    posMovesList.ElementAt(i).score = score;

                    //check for highscores and set best moves
                    if (map.black)
                    {
                        if (score >= highScore)
                        {
                            //Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) + " ; " + (bufferMove.q + fieldRadius) + " " + score);
                            highScore = score;
                            bestMove = bufferMove;
                            bestMove.score = score;
                        }
                    }
                    else
                    {
                        if (score <= highScore)
                        {
                            //Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) + " ; " + (bufferMove.q + fieldRadius) + " " + score);
                            highScore = score;
                            bestMove = bufferMove;
                            bestMove.score = score;
                        }
                    }
                }

                if (map.black && highScore == -1500)
                    Console.WriteLine("Enemy Red will win in " + map.aiDepth + " moves");
                else if (!map.black && highScore == 1500)
                    Console.WriteLine("Enemy Black will win in " + map.aiDepth + " moves");
                else
                {
                    //remove "best move" from last iteration and set the new one
                    map.swapMoves(4,3);
                    map.myField[(bestMove.r + fieldRadius), (bestMove.q + fieldRadius)] = 4;
                }
                    
                //Move ordering
                if(map.black)
                    posMovesList = posMovesList.OrderBy(q => q.score).ToList();
                else
                    posMovesList = posMovesList.OrderByDescending(buffer => buffer.score).ToList();

                //increase ai depth
                map.aiDepth++;
            }
        }

        public int alphaBeta(Move move, int[,] scoreField, int depth, int alpha, int beta, bool black)
        {
            //first check for win
            int win = check.checkWin(scoreField);

            if (win == 1)
                return 2000;

            if (win == 2)
                return -2000;

            //depth = 0 --> evaluate position 
            if (depth == 0)
                return evalPos(scoreField);

            //return evalPos(scoreField);
            //Console.WriteLine("checking move: " + (move.r +fieldRadius) + ";" + (move.q + fieldRadius));

            if (black)
            {
                scoreField = map.checkPosMoves(scoreField);

                //get all new possible moves
                List<Move> posAB = map.getPosMoves(scoreField);

                int maxEval = -9999;
                int eval = 0;

                for (int i = 0; i < posAB.Count; i++)
                {
                    Move bufferMove = posAB.ElementAt(i);

                    //Make a deep copy
                    int[,] bufferField = new int[20, 20];
                    bufferField = scoreField.Clone() as int[,];

                    //set the new possible move
                    bufferField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 1;

                    eval = alphaBeta(bufferMove, bufferField, depth - 1, alpha, beta, false);

                    //Console.WriteLine("Move: " + (bufferMove.r + fieldRadius) + " ; " + (bufferMove.q + fieldRadius) + " " + eval)
                    maxEval = Math.Max(maxEval, eval);

                    //alphaBeta
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }

                return maxEval;
            }
            else
            {
                scoreField = map.checkPosMoves(scoreField);

                //get all new possible moves
                List<Move> posAB = map.getPosMoves(scoreField);

                int minEval = 9999;
                int eval = 0;
                for (int i = 0; i < posAB.Count; i++)
                {
                    Move bufferMove = posAB.ElementAt(i);

                    //Make a deep copy
                    int[,] bufferField = new int[20, 20];
                    bufferField = scoreField.Clone() as int[,];

                    //set the new possible move
                    bufferField[bufferMove.r + fieldRadius, bufferMove.q + fieldRadius] = 2;

                    eval = alphaBeta(bufferMove, bufferField, depth - 1, alpha, beta, true);

                    minEval = Math.Min(minEval, eval);

                    //alphaBeta
                    beta = Math.Min(beta, eval);
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

            //give reward, if the own row is "better"
            if (rbestrow > bbestrow-1)
                reward += -200;
            else if (rbestrow-1 < bbestrow)
                reward += 200;
            else if (rbestrow > bbestrow)
                reward += -100;
            else if (rbestrow < bbestrow)
                reward += 100;

            bbestrow = 0;
            rbestrow = 0;

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

            //give reward, if the own row is "better"
            if (rbestrow > bbestrow - 1)
                reward += -200;
            else if (rbestrow - 1 < bbestrow)
                reward += 200;
            else if (rbestrow > bbestrow)
                reward += -100;
            else if (rbestrow < bbestrow)
                reward += 100;

            bbestrow = 0;
            rbestrow = 0;

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

            //give reward, if the own row is "better"
            if (rbestrow > bbestrow - 1)
                reward += -200;
            else if (rbestrow - 1 < bbestrow)
                reward += 200;
            else if (rbestrow > bbestrow)
                reward += -100;
            else if (rbestrow < bbestrow)
                reward += 100;

            //Console.WriteLine("Bestrow: " + bestrow + " for " + black);

            return reward;
        }
    }
}
