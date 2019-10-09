using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Adantino_2
{
    public partial class Form1 : Form
    {
        Map myMap;
        int fieldRadius = 9;
        int fieldSize = 27;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(Map bufferMap)
        {
            InitializeComponent();
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Click += new EventHandler(panel1_Click);
            Controls.Add(panel1);
            button1.Visible = false;
            myMap = bufferMap;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Console.WriteLine("Paint Panel 1");
            Graphics g = e.Graphics;
            drawField(g);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("Click Panel 1");

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

            int win = myMap.makeMove((int)r, (int)q); // -1 invalid move, 0 normal move, 1 Black wins, 2 Red wins

            if (win == 0)
            {
                label2.Text = "Move: " + myMap.moveCounter;
                //move done
                if (myMap.black)
                {
                    label1.ForeColor = Color.Black;
                    label1.Text = "Blacks turn";
                }
                else
                {
                    label1.ForeColor = Color.Red;
                    label1.Text = "Reds turn";
                }

                button1.Visible = true;
                this.Refresh();
            }
            else if (win == 1)
            {
                //Black wins
                label1.ForeColor = Color.Black;
                label1.Text = "Black wins!";
                this.Refresh();
            }
            else if (win == 2)
            {
                //Red wins
                label1.ForeColor = Color.Red;
                label1.Text = "Red wins!";
                this.Refresh();
            }
            else
            {
                //invalid move
                label1.Text += " / Invalid move!";
                this.Refresh();
            }
            /*
            g.DrawString("Clicked to Pos: " + curserPoint.X + " Y: " + curserPoint.Y, this.Font, Brushes.Black, 0f, 0f);
            g.DrawString("q: " + q, this.Font, Brushes.Black, 0f, 20f);
            g.DrawString("r: " + r, this.Font, Brushes.Black, 0f, 40f);
            */
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

        public void drawField(Graphics g)
        {
           
            for (int r = -(fieldRadius); r<= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r- fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    double coordX = fieldSize * (Math.Sqrt(3) * q + Math.Sqrt(3) / 2 * r);
                    double coordY = fieldSize * ((1.5f) * r);

                    coordX += (panel1.Width / 2);
                    coordY += (panel1.Height / 2);

                    PointF[] buffer = GetDrawPoints((float)coordX, (float)coordY, (int)fieldSize);
                    
                    Pen myPen = new Pen(Color.Blue, 1);
                    int[,] bufferMap = myMap.myField;
    
                    if(bufferMap[r+fieldRadius,q+fieldRadius] == 1)
                    {
                        SolidBrush myBrush = new SolidBrush(Color.Black);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }
                    else if (bufferMap[r + fieldRadius, q+fieldRadius] == 2)
                    {
                        SolidBrush myBrush = new SolidBrush(Color.Red);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }
                    else if (bufferMap[r + fieldRadius, q + fieldRadius] == 3)
                    {
                        SolidBrush myBrush = new SolidBrush(Color.Gray);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }
                    else if (bufferMap[r + fieldRadius, q + fieldRadius] == 4)
                    {
                        SolidBrush myBrush = new SolidBrush(Color.Goldenrod);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }
                    else
                    {
                        SolidBrush myBrush = new SolidBrush(Color.BurlyWood);
                        // Draw ellipse to screen.
                        g.FillPolygon(myBrush, buffer);
                    }

                    g.DrawPolygon(myPen, buffer);

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    g.DrawString((r + fieldRadius) + ";" + (q + fieldRadius), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10, (float)coordY - (float)fieldSize + 8);
                    //Thread.Sleep(15);
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
            if(myMap.moveCounter >= 1)
            {
                int[,] bufferField = myMap.moveList.ElementAt(myMap.moveCounter - 1).Clone() as int[,];
                myMap.myField = bufferField;
                myMap.moveList.RemoveAt(myMap.moveCounter);

                myMap.moveCounter--;
                if(myMap.moveCounter == 0)
                {
                    button1.Visible = false;
                }

                myMap.black = !myMap.black;

                label2.Text = "Move: " + myMap.moveCounter;

                if (myMap.black)
                {
                    label1.ForeColor = Color.Black;
                    label1.Text = "Blacks turn";
                }
                else
                {
                    label1.ForeColor = Color.Red;
                    label1.Text = "Reds turn";
                }

                this.Refresh();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Restart clicked");
            if (myMap.moveCounter >= 1)
            {
                myMap.initField();
                this.Refresh();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            myMap.aiOne = !myMap.aiOne;
            //this.Refresh();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            myMap.aiTwo = !myMap.aiTwo;
            //this.Refresh();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
