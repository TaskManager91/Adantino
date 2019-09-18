using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Adantino_2
{
    public partial class Form1 : Form
    {
        int fieldRadius = 9;
        int fieldSize = 27;
        bool black = false;

        int[,] myField = new int[20, 20];

        List<int[,]> moveList = new List<int[,]>();
        int moveCounter = 0;
        public Form1()
        {
            InitializeComponent();
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Click += new EventHandler(panel1_Click);
            this.Controls.Add(panel1);

            //filling the Array with -1
            for(int i = 0; i<20; i++)
            {
                for(int y = 0; y<20; y++)
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Console.WriteLine("Paint Panel 1");
            Graphics g = e.Graphics;

            drawField(fieldRadius, fieldSize, g);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Click Panel 1");

            Point curserPoint = panel1.PointToClient(Cursor.Position);

            curserPoint.X -= (panel1.Width / 2);
            curserPoint.Y -= (panel1.Height / 2);

            Graphics g = this.CreateGraphics();

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            //calculate q+r and dont forget to round !
            double q = (0.57735026919d * curserPoint.X - 0.3333333333d * curserPoint.Y) / fieldSize;
            double r = (0.66666666d * curserPoint.Y) / fieldSize;
            PointF roundedPoint = roundCoord(q, r);
            q = roundedPoint.X;
            r = roundedPoint.Y;

            if(myField[(int)r + fieldRadius, (int)q + fieldRadius] != 3)
            {
                g.DrawString("invalid move!" + curserPoint.Y, this.Font, Brushes.Black, 0f, 0f);
            }
            else
            {
                if (black)
                {
                    myField[(int)r + fieldRadius, (int)q + fieldRadius] = 1;
                    //Console.WriteLine("Pos: " + ((int)r + fieldRadius) + " ; " + ((int)q + fieldRadius) + " set to Black");
                }
                else
                {
                    myField[(int)r + fieldRadius, (int)q + fieldRadius] = 2;
                    Console.WriteLine("Pos: " + ((int)r + fieldRadius) + " ; " + ((int)q + fieldRadius) + " set to Red");
                }
                checkPosMoves();

                this.Refresh();

                //Make a deep copy
                int[,] bufferField = myField.Clone() as int[,];

                //add move to moveList
                moveList.Add(bufferField);
                moveCounter++;

                black = !black;
            }
            /*
            g.DrawString("Clicked to Pos: " + curserPoint.X + " Y: " + curserPoint.Y, this.Font, Brushes.Black, 0f, 0f);
            g.DrawString("q: " + q, this.Font, Brushes.Black, 0f, 20f);
            g.DrawString("r: " + r, this.Font, Brushes.Black, 0f, 40f);
            */
        }

        public void checkPosMoves()
        {
            for (int q = -(fieldRadius); q <= fieldRadius; q++)
            {
                int r1 = Math.Max(-fieldRadius, -q - fieldRadius);
                int r2 = Math.Min(fieldRadius, -q + fieldRadius);
                for (int r = r1; r <= r2; r++)
                {
                    if (myField[r + fieldRadius, q + fieldRadius] == 1)
                    {
                        //Black: nothing todo
                    }
                    else if (myField[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        //Red: nothing todo
                    }
                    else if (myField[r + fieldRadius, q + fieldRadius] == 3 || myField[r + fieldRadius, q + fieldRadius] == 0 )
                    {
                        int checker = 0;
                        int x = r + fieldRadius;
                        int y = q + fieldRadius;

                        if( x+1 <= 20)
                        {
                            if (myField[x + 1, y] == 1 || myField[x + 1, y] == 2)
                                checker++;
                        }
                        
                        if( x+1 <= 20 && y-1 >= 0)
                        {
                            if (myField[x + 1, y - 1] == 1 || myField[x + 1, y - 1] == 2)
                                checker++;
                        }

                        if (y - 1 >= 0)
                        {
                            if (myField[x, y - 1] == 1 || myField[x, y - 1] == 2)
                                checker++;
                        }

                        if (y + 1 >= 0)
                        {
                            if (myField[x, y + 1] == 1 || myField[x, y + 1] == 2)
                                checker++;
                        }

                        if (x - 1 >= 0)
                        {
                            if (myField[x - 1, y] == 1 || myField[x - 1, y] == 2)
                                checker++;
                        }

                        if (y + 1 <= 20 && x - 1 >= 0)
                        {
                            if (myField[x - 1, y + 1] == 1 || myField[x - 1, y + 1] == 2)
                                checker++;
                        }

                        if(checker >= 2)
                        {
                            myField[r + fieldRadius, q + fieldRadius] = 3;
                        }
                        else
                        {
                            myField[r + fieldRadius, q + fieldRadius] = 0;
                        }
                    }
                }
            }
        }

        public PointF roundCoord(double q, double r)
        {
            PointF point = new PointF();

            int qi = (int)(Math.Round(q));
            int ri = (int)(Math.Round(r));
            int si = (int)(Math.Round(-q-r));
            double q_diff = Math.Abs(qi - q);
            double r_diff = Math.Abs(ri - r);
            double s_diff = Math.Abs(si - (-q-r));
            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else
                if (r_diff > s_diff)
                {
                    ri = -qi - si;
                }
                else
                {
                    si = -qi - ri;
                }

            point.X = qi;
            point.Y = ri;

            return point;
        }

        public void drawField(int sizeField, double size, Graphics g)
        {
            for (int q = -(sizeField); q<=sizeField; q++)
            {
                int r1 = Math.Max(-sizeField, -q- sizeField);
                int r2 = Math.Min(sizeField, -q + sizeField);
                for (int r = r1; r <= r2; r++)
                {
                    double coordX = size * (Math.Sqrt(3) * q + Math.Sqrt(3) / 2 * r);
                    double coordY = size * ((1.5f) * r);

                    coordX += (panel1.Width / 2);
                    coordY += (panel1.Height / 2);

                    PointF[] buffer = GetDrawPoints((float)coordX, (float)coordY, (int) size);
                    
                    Pen myPen = new Pen(Color.Blue, 1);
    
                    if(myField[r+fieldRadius,q+fieldRadius] == 1)
                    {
                        SolidBrush myBrush = new SolidBrush(Color.Black);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }
                    else if (myField[r + fieldRadius, q+fieldRadius] == 2)
                    {
                        SolidBrush myBrush = new SolidBrush(Color.Red);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }
                    else if (myField[r + fieldRadius, q + fieldRadius] == 3)
                    {
                        SolidBrush myBrush = new SolidBrush(Color.Gray);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }

                    g.DrawPolygon(myPen, buffer);

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    g.DrawString((q + fieldRadius) + ";" + (r + fieldRadius), this.Font, Brushes.Green, (float)coordX - (float)size + 10, (float)coordY - (float)size + 8);

                }
            }
        }

        public PointF[] GetDrawPoints(float x, float y, int size)
        {
            PointF[] shape = new PointF[6];

            //Create 6 points
            for (int i = 0; i < 6; i++)
            {
                float angle_deg = 60 * i -30;
                float angle_rad = (float)Math.PI / 180 * angle_deg;
                shape[i] = new PointF((x + size * (float)Math.Cos(angle_rad)), (y + size * (float)Math.Sin(angle_rad)));
            }

            return shape;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Undo clicked");

            int[,] bufferField = moveList.ElementAt(moveCounter-1).Clone() as int[,];
            myField = bufferField;
            moveList.RemoveAt(moveCounter);

            moveCounter--;

            this.Refresh();

            black = !black;

        }
    }
}
