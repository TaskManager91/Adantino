using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adantino_2
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
                    checker += map.checkNeighbors(r, q, 5, checkField);

                if (checker == 0 && currentPlayer == 2)
                    checker += map.checkNeighbors(r, q, 6, checkField);

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
    }
}
