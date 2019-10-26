using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adantino
{
    class CheckWin
    {
        const int fieldRadius = 9;

        private Map map;

        public CheckWin(Map bufferMap)
        {
            map = bufferMap;
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

            int bufferWin = 0;
            bufferWin = checkTrapped(bufferField);

            if (bufferWin != 0)
            {
                int[,] bufferFieldTwo = new int[20, 20];
                bufferFieldTwo = checkField.Clone() as int[,];
                win = checkDeepTrapped(bufferFieldTwo);
            }
           

            return win;
        }

        public int checkDeepTrapped(int[,] checkField)
        {
            int win = 0;

            List<Move> prisonedFields = new List<Move>();

            for (int r = -(fieldRadius); r <= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r - fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    if (checkField[r + fieldRadius, q + fieldRadius] == 1 || checkField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        int checker = 0;

                        checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, 1, checkField);
                        checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, 2, checkField);

                        if (checker == 6)
                        {
                            //Console.WriteLine("Trapped Fields: " + (r + fieldRadius) + " ; " + (q + fieldRadius) + " " + checker);
                            Move bufferMove = new Move(r + fieldRadius, q + fieldRadius);
                            prisonedFields.Add(bufferMove);
                        }
                    }
                }
            }

            

            for (int i = 0; i < prisonedFields.Count; i++)
            {
                Move bufferMove = prisonedFields.ElementAt(i);
                int checkPlayer = checkField[bufferMove.r, bufferMove.q];
                bool trapped = false;

                int[,] bufferField = new int[20, 20];
                bufferField = checkField.Clone() as int[,];

                trapped = badBFS(bufferMove, bufferField);

                if (trapped)
                {
                    //Console.WriteLine("GOT A TRAPPED FIELD");
                    if (checkPlayer == 1)
                        win = 2;
                    else
                        win = 1;
                }
            }
            return win;
        }

        public bool badBFS(Move move, int[,] checkField)
        {
            bool trapped = true;

            List<Move> queue = new List<Move>();
            queue.Add(move);
            List<Move> visitedMoves = new List<Move>();

            int currentPlayer = checkField[move.r, move.q];
            int enemy = 0;

            if (currentPlayer == 1)
                enemy = 2;
            else
                enemy = 1;

            while (queue.Count > 0)
            {
                Move activeMove = new Move(0, 0);
                activeMove = queue.ElementAt(0);
                visitedMoves.Add(activeMove);
                queue.RemoveAt(0);

                List<Move> myNeighbors = new List<Move>();

                myNeighbors = map.getMyEnemyNeighbors(activeMove.r, activeMove.q, checkField, enemy);

                for (int i = 0; i < myNeighbors.Count; i++)
                {
                    Move bufferMove = myNeighbors.ElementAt(i);

                    bool contains = false;

                    contains = containsVisited(visitedMoves, bufferMove);

                    if (contains)
                    {
                        //i've seen this place before ...
                    }
                    else
                    {
                        if (bufferMove.q == 0)
                            return false;

                        if (bufferMove.r == 0)
                            return false;

                        if (checkField[bufferMove.r, bufferMove.q] == 3)
                            return false;

                        if (checkField[bufferMove.r, bufferMove.q] == 4)
                            return false;

                        if (checkField[bufferMove.r, bufferMove.q] == 0)
                            return false;

                        if (checkField[bufferMove.r, bufferMove.q] == -1)
                            return false;

                        queue.Add(bufferMove);
                    }
                }
            }
            return trapped;
        }

        public bool containsVisited(List<Move> visitedMoves, Move move)
        {
            bool contains = false;
            for (int i = 0; i < visitedMoves.Count; i++)
            {
                Move bufferMove = visitedMoves.ElementAt(i);
                if (bufferMove.q == move.q)
                {
                    if (bufferMove.r == move.r)
                        return true;
                }
            }
            return contains;
        }

        public int checkTrapped(int[,] checkField)
        {
            int win = 0;

            for (int r = -(fieldRadius); r <= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r - fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    if (checkField[r + fieldRadius, q + fieldRadius] == 1 || checkField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        int currentPlayer = checkField[r + fieldRadius, q + fieldRadius];

                        //check if at least one neighbor is free
                        int checker = 0;

                        checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, 0, checkField);

                        if (q + fieldRadius == 0)
                            checker++;

                        if (r + fieldRadius == 0)
                            checker++;

                        if (checker == 0)
                            checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, -1, checkField);

                        if (checker == 0)
                            checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, 3, checkField);

                        if (checker == 0)
                            checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, 4, checkField);

                        if (checker == 0 && currentPlayer == 1)
                            checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, 5, checkField);

                        if (checker == 0 && currentPlayer == 2)
                            checker += map.checkNeighbors(r + fieldRadius, q + fieldRadius, 6, checkField);

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
                            checker += checkTrappedHelper(r + fieldRadius, q + fieldRadius, checkField, currentPlayer);
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

        public int checkTrappedHelper(int r, int q, int[,] checkField, int currentPlayer)
        {
            //Console.WriteLine("need to dig deeper!");
            //int currentPlayer = checkField[r, q];

            int win = 0;

            int enemy = 0;

            if (currentPlayer == 1)
                enemy = 2;
            else
                enemy = 1;

            List<Move> myNeighbors = new List<Move>();

            myNeighbors = map.getMyNeighbors(r, q, checkField, currentPlayer);

            //Console.WriteLine("R,q: " + r + " " + q + " checking count of neighbors: " + myNeighbors.Count);

            for (int i = 0; i < myNeighbors.Count; i++)
            {

                Move move = myNeighbors.ElementAt(i);
                //Console.WriteLine("Checking Neighbor (r,q): " + move.r + " " + move.q);
                //check if at least one neighbor of Neighbor is free
                int checker = 0;
                checker += map.checkNeighbors(move.r, move.q, 0, checkField);

                if (move.r == 0 || move.q == 0)
                    checker++;

                if(checker == 0)
                    checker += map.checkNeighbors(move.r, move.q, -1, checkField);

                if (checker == 0)
                    checker += map.checkNeighbors(move.r, move.q, 3, checkField);

                if (checker == 0)
                    checker += map.checkNeighbors(move.r, move.q, 4, checkField);

                if (checker == 0 && currentPlayer == 1)
                    checker += map.checkNeighbors(move.r, move.q, 5, checkField);

                if (checker == 0 && currentPlayer == 2)
                    checker += map.checkNeighbors(move.r, move.q, 6, checkField);

                //Jaay, free field => set to 5 for black or 6 for Red (checked and free) else dig deeper
                if (checker > 0)
                {
                    return 1;
                }
                else
                {
                    checkField[move.r, move.q] = 99; //Field checked -> ignore in deeper steps to prevent stack overflow
                    checker += checkTrappedHelper(move.r, move.q, checkField, currentPlayer);
                }

                if (checker > 0)
                    return 1;
                else
                    return 0;
            }

            return win;
        }
    }
}
