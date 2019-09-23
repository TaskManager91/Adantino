﻿using System;
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

        public void checkPosMoves()
        {
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (myField[r + fieldRadius, q + fieldRadius] == 3 || myField[r + fieldRadius, q + fieldRadius] == 0)
                    {
                        int y = r + fieldRadius;
                        int x = q + fieldRadius;

                        //check if at least two neighbors are set
                        int checker = checkNeighbors(y, x, 1, 2);

                        if (checker >= 2)
                            myField[r + fieldRadius, q + fieldRadius] = 3;
                        else
                            myField[r + fieldRadius, q + fieldRadius] = 0;
                    }
                }
            }
        }

        public int checkNeighbors(int y, int x, int typeOne, int typeTwo)
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
            int win = 0;
            bool[,] visited = new bool[20, 20];
            bool[,] prisoned = new bool[20, 20];

            //check for prisoned fields
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (myField[r + fieldRadius, q + fieldRadius] == 1 || myField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        int y = r + fieldRadius;
                        int x = q + fieldRadius;

                        if(visited[y,x] == false)
                        {
                            prisoned[y, x] = CheckPrisonedRelatives(y, x, visited, prisoned);
                        }
                    }
                }
            }
            return win;
        }

        public bool CheckPrisonedRelatives(int y, int x, bool[,] visited, bool[,] prisoned)
        {

            int freeFields = 0;
            int myType = myField[y, x];
            visited[y, x] = true;

            if (checkNeighbors(y, x, 3, 99) >= 1)
            {
                //found free field -> finish
                return false;
            }
            else
            {
                //check Relatives
                if (y + 1 <= 20)
                    if (myField[y + 1, x] == myType)
                        prisoned[y, x] = CheckPrisonedRelatives(y, x, visited, prisoned);

                if (y + 1 <= 20 && x - 1 >= 0)
                    if (myField[y + 1, x - 1] == myType)
                        freeFields++;

                if (x - 1 >= 0)
                    if (myField[y, x - 1] == myType)
                        freeFields++;

                if (x + 1 >= 0)
                    if (myField[y, x + 1] == myType)
                        freeFields++;

                if (y - 1 >= 0)
                    if (myField[y - 1, x] == myType)
                        freeFields++;

                if (x + 1 <= 20 && y - 1 >= 0)
                    if (myField[y - 1, x + 1] == myType)
                        freeFields++;
            }

            return false;
        }

        public bool makeMove(int r, int q)
        {
            if (myField[(int)r + fieldRadius, (int)q + fieldRadius] != 3)
            {
                //Invalid move! -> not playable
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

                //Check the Field again for possible moves
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