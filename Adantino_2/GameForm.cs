using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Adantino
{
    public partial class GameForm : Form
    {
        Map myMap;
        AI ai;

        int fieldRadius = 9;
        int fieldSize = 27;

        int aiTime = 0;
        int currentAiTime = 0;
        Thread abThread;
        Thread abTimerThread;
        char[] notationRev = new char[19];
        char[] notation = new char[19];
        bool havannah;
        bool intern;

        public GameForm()
        {
            InitializeComponent();
        }

        public GameForm(Map bufferMap)
        {
            InitializeComponent();
            //this.DoubleBuffered = true;

            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Click += new EventHandler(panel1_Click);
            Controls.Add(panel1);

            undo_button.Visible = false;

            //store the map
            myMap = bufferMap;

            //create new AI with the map
            ai = new AI(myMap);
            abThread = new Thread(ai.alphaBetaStart);

            //necessary for the havannah field notation
            char[] bufferNotationRev = { 'S','R', 'Q', 'P', 'O', 'N', 'M', 'L', 'K', 'J', 'I', 'H','G','F', 'E', 'D', 'C', 'B', 'A' };
            notationRev = bufferNotationRev;

            char[] bufferNotation = { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s'};
            notation = bufferNotation;

            //Show havannah or internal field notation
            havannah = true;
            intern = false;
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

            //start in the middle of the Panel
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

            int win = myMap.makeMove((int) r, (int) q); // -1 invalid move, 0 normal move, 1 Black wins, 2 Red wins

            if (win == 0)
            {
                undo_button.Visible = true;
                this.Refresh();
                //move done

                if (myMap.black)
                {
                    if (myMap.blackAI)
                    {
                        turn_label.ForeColor = Color.Black;
                        turn_label.Text = "Black AI running ...";
                        this.Refresh();

                        // START AlphaBeta
                        Thread abThread;
                        abThread = new Thread(ai.alphaBetaStart);
                        abThread.IsBackground = true;
                        abThread.Start();


                        if (myMap.moveCounter <= 5)
                            Thread.Sleep(1000);
                        else
                            Thread.Sleep(5000);

                        //Time is over -> kill it
                        abThread.Abort();
                    }

                    turn_label.ForeColor = Color.Black;
                    turn_label.Text = "Blacks turn";
                    this.Refresh();
                }
                else
                {
                    if (myMap.redAI)
                    {
                        turn_label.ForeColor = Color.Red;
                        turn_label.Text = "Red AI running ...";
                        this.Refresh();

                        // START AlphaBeta
                        Thread abThread;
                        abThread = new Thread(ai.alphaBetaStart);
                        abThread.IsBackground = true;
                        abThread.Start();


                        if (myMap.moveCounter <= 5)
                            Thread.Sleep(1000);
                        else
                            Thread.Sleep(5000);

                        //Time is over -> kill it
                        abThread.Abort();
                    }

                    turn_label.ForeColor = Color.Red;
                    turn_label.Text = "Reds turn";
                    this.Refresh();
                }
            }
            else if (win == 1)
            {
                //Black wins
                turn_label.ForeColor = Color.Black;
                turn_label.Text = "Black wins!";
                this.Refresh();
            }
            else if (win == 2)
            {
                //Red wins
                turn_label.ForeColor = Color.Red;
                turn_label.Text = "Red wins!";
                this.Refresh();
            }
            else
            {
                //invalid move
                turn_label.Text += " / Invalid move!";
                this.Refresh();
            }

            /*
            g.DrawString("Clicked to Pos: " + curserPoint.X + " Y: " + curserPoint.Y, this.Font, Brushes.Black, 0f, 0f);
            g.DrawString("q: " + q, this.Font, Brushes.Black, 0f, 20f);
            g.DrawString("r: " + r, this.Font, Brushes.Black, 0f, 40f);
            */
        }

        //round the coords 
        public PointF roundCoord(double q, double r)
        {
            PointF point = new PointF();

            //from https://www.redblobgames.com/grids/hexagons/ 

            int qi = (int) (Math.Round(q));
            int ri = (int) (Math.Round(r));
            int si = (int) (Math.Round(-q - r));
            double q_diff = Math.Abs(qi - q);
            double r_diff = Math.Abs(ri - r);
            double s_diff = Math.Abs(si - (-q - r));
            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else if (r_diff > s_diff)
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
            //start timer ... rendering is slow
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Pen myPen = new Pen(Color.Blue, 1);

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            int[,] bufferMap = myMap.myField;

            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush redBrush = new SolidBrush(Color.Red);
            SolidBrush goldBrush = new SolidBrush(Color.Goldenrod);
            SolidBrush greyBrush = new SolidBrush(Color.Gray);
            SolidBrush brownBrush = new SolidBrush(Color.BurlyWood);

            this.Font = new Font("Arial", 9, FontStyle.Bold);

            //run through the "playable" field
            for (int r = -(fieldRadius); r <= fieldRadius; r++)
            {
                int q1 = Math.Max(-fieldRadius, -r - fieldRadius);
                int q2 = Math.Min(fieldRadius, -r + fieldRadius);
                for (int q = q1; q <= q2; q++)
                {
                    int s = -q - r;

                    //calculate coords to draw
                    double coordX = fieldSize * (Math.Sqrt(3) * q + Math.Sqrt(3) / 2 * r);
                    double coordY = fieldSize * ((1.5f) * r);

                    coordX += (panel1.Width / 2);
                    coordY += (panel1.Height / 2);

                    //get the corner Points of each hexagon
                    PointF[] buffer = GetDrawPoints((float) coordX, (float) coordY, (int) fieldSize);

                    if (bufferMap[r + fieldRadius, q + fieldRadius] == 1)
                    {
                        // Black Player
                        g.FillPolygon(blackBrush, buffer);
                        g.DrawPolygon(myPen, buffer);
                       
                        if(havannah)
                            g.DrawString((notationRev[s + fieldRadius]) + " " + (r + fieldRadius + 1), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 12, (float)coordY - (float)fieldSize + 15);
                        
                        if(intern)
                            g.DrawString((r + fieldRadius) + ";" + (q + fieldRadius), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10, (float)coordY - (float)fieldSize + 8);
                          
                    }
                    else if (bufferMap[r + fieldRadius, q + fieldRadius] == 2)
                    {
                        // Red Player
                        g.FillPolygon(redBrush, buffer);
                        g.DrawPolygon(myPen, buffer);

                        if (havannah)
                            g.DrawString((notationRev[s + fieldRadius]) + " " + (r + fieldRadius + 1), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 12, (float)coordY - (float)fieldSize + 15);

                        if (intern)
                            g.DrawString((r + fieldRadius) + ";" + (q + fieldRadius), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10, (float)coordY - (float)fieldSize + 8);
                    }
                    else if (bufferMap[r + fieldRadius, q + fieldRadius] == 3)
                    {
                        // Possible move
                        g.FillPolygon(greyBrush, buffer);
                        g.DrawPolygon(myPen, buffer);

                        if (havannah)
                            g.DrawString((notationRev[s + fieldRadius]) + " " + (r + fieldRadius + 1), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 12, (float)coordY - (float)fieldSize + 15);

                        if (intern)
                            g.DrawString((r + fieldRadius) + ";" + (q + fieldRadius), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10, (float)coordY - (float)fieldSize + 8);
                    }
                    else if (bufferMap[r + fieldRadius, q + fieldRadius] == 4)
                    {

                        // Suggested move
                        g.FillPolygon(goldBrush, buffer);
                        g.DrawPolygon(myPen, buffer);

                        if (havannah)
                            g.DrawString((notationRev[s + fieldRadius]) + " " + (r + fieldRadius + 1), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 12, (float)coordY - (float)fieldSize + 15);

                        if (intern)
                            g.DrawString((r + fieldRadius) + ";" + (q + fieldRadius), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10, (float)coordY - (float)fieldSize + 8);

                    }
                    else
                    {
                        // empty field
                        g.FillPolygon(brownBrush, buffer); // <- comment this line out for better render performance
                    }

                    g.DrawPolygon(myPen, buffer); // <- comment this line out for better render performance
                    //g.DrawString((notationRev[s + fieldRadius]) + " " + (r + fieldRadius + 1), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 12, (float)coordY - (float)fieldSize + 15);
                    //Thread.Sleep(15);
                }
            }
            
            
            /*
            // CHECK whole array (boundaries etc.)
            for (int r =0; r < 20; r++)
            {
                for (int q = 0; q < 20; q++)
                {
                    double coordX = fieldSize * (Math.Sqrt(3) * q + Math.Sqrt(3) / 2 * r);
                    double coordY = fieldSize * ((1.5f) * r);

                    coordX -= 150;
                    coordY += 150;

                    PointF[] buffer = GetDrawPoints((float)coordX, (float)coordY, (int)fieldSize);

                    if (bufferMap[r, q] == 1)
                    {
                        // Black Player
                        g.FillPolygon(blackBrush, buffer);
                        g.DrawPolygon(myPen, buffer);
                    }
                    else if (bufferMap[r, q] == 2)
                    {
                        // Red Player
                        g.FillPolygon(redBrush, buffer);
                        g.DrawPolygon(myPen, buffer);
                    }
                    else if (bufferMap[r, q] == 3)
                    {
                        // Possible move
                        g.FillPolygon(greyBrush, buffer);
                        g.DrawPolygon(myPen, buffer);
                        g.DrawString((r) + ";" + (q), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10,
                            (float)coordY - (float)fieldSize + 8);
                    }
                    else if (bufferMap[r, q] == 4)
                    {

                        // Suggested move
                        g.FillPolygon(goldBrush, buffer);
                        g.DrawPolygon(myPen, buffer);
                        g.DrawString((r) + ";" + (q), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10,
                            (float)coordY - (float)fieldSize + 8);
                    }
                    else if (bufferMap[r, q] == 0)
                    {
                        // empty field
                        g.FillPolygon(brownBrush, buffer);
                    }

                    g.DrawPolygon(myPen, buffer);

                    g.DrawString((r) + ";" + (q), this.Font, Brushes.Aqua, (float)coordX - (float)fieldSize + 10, (float)coordY - (float)fieldSize + 8);
                    //Thread.Sleep(15);
                }
            }
            */

            stopwatch.Stop();
            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
            int time = Convert.ToInt32(stopwatchElapsed.TotalMilliseconds);

            //redraw the labels
            move_label.Text = "Move: " + myMap.moveCounter;
            ai_depth_label.Text = "AI depth: " + myMap.aiDepth;
            ai_lt_label.Text = "Last AI time: " + aiTime + " ms";
            ai_ct_label.Text = "current time: " + currentAiTime + " ms";

            render_label.Text = "Rendertime: " + time + " ms";

        }

        public PointF[] GetDrawPoints(float x, float y, int size)
        {
            PointF[] shape = new PointF[6];

            //Create 6 points
            for (int i = 0; i < 6; i++)
            {
                //from https://www.redblobgames.com/grids/hexagons/ 
                float angle_deg = 60 * i - 30;
                float angle_rad = (float) Math.PI / 180 * angle_deg;
                shape[i] = new PointF((x + size * (float) Math.Cos(angle_rad)),
                    (y + size * (float) Math.Sin(angle_rad)));
            }

            return shape;
        }

        private void undo_button_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Undo clicked");

            if (myMap.moveCounter >= 1)
            {
                //set field to the previous state
                int[,] bufferField = myMap.moveList.ElementAt(myMap.moveCounter - 1).Clone() as int[,];
                myMap.myField = bufferField;

                //remove the last move
                myMap.moveList.RemoveAt(myMap.moveCounter);
                myMap.moveCounter--;

                if (myMap.moveCounter == 0)
                {
                    //No moves made -> make the undo button invisible
                    undo_button.Visible = false;
                }

                //swap the Player
                myMap.black = !myMap.black;

                move_label.Text = "Move: " + myMap.moveCounter;

                if (myMap.black)
                {
                    turn_label.ForeColor = Color.Black;
                    turn_label.Text = "Blacks turn";
                }
                else
                {
                    turn_label.ForeColor = Color.Red;
                    turn_label.Text = "Reds turn";
                }

                this.Refresh();
            }
        }

        private void restart_button_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Restart clicked");
            if (myMap.moveCounter >= 1)
            {
                //just call initField to reset the Game
                myMap.initField();
                this.Refresh();
            }
        }

        private void kill_ai_button_Click(object sender, EventArgs e)
        {
            if (!myMap.abReady)
            {
                //Kill the Threads 
                myMap.abReady = true;
                abThread.Abort();
                abTimerThread.Abort();
            }

        }

        private void start_ai_button_Click(object sender, EventArgs e)
        {
            myMap.abReady = false;

            //START Timer + Inspector
            abTimerThread = new Thread(abThreadInspector);
            abTimerThread.IsBackground = true;
            abTimerThread.Start();

            // START AlphaBeta
            abThread = new Thread(ai.alphaBetaStart);
            abThread.IsBackground = true;
            abThread.Start();
        }

        public void abThreadInspector()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int bufferDepth = 4;
            Console.WriteLine("START AI at move: " + myMap.moveCounter);
            int sleep = 0;
            int lastTime = 0;

            do
            {
                //wait a bit
                Thread.Sleep(50);

                TimeSpan elapsed = stopwatch.Elapsed;
                currentAiTime = Convert.ToInt32(elapsed.TotalMilliseconds);

                //if the ai reached a new depth
                if (myMap.aiDepth >= bufferDepth)
                {

                    if (lastTime == 0)
                        lastTime = currentAiTime;
                    else
                        lastTime = currentAiTime - lastTime;

                    Console.WriteLine("Depth: " + myMap.aiDepth);
                    Console.WriteLine("Time: " + lastTime);
                    redrawPanelSafe();
                    aiTime = currentAiTime;
                    currentAiTime = 0;
                    bufferDepth++;
                }

                if(sleep == 4)
                {
                    redrawLabelsSafe();
                    sleep = 0;
                }
                
                sleep++;
            } while (!myMap.abReady);


            stopwatch.Stop();
            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
            aiTime = Convert.ToInt32(stopwatchElapsed.TotalMilliseconds);

            redrawPanelSafe();
            redrawLabelsSafe();
        }

        //necessary for threading
        public void redrawLabelsSafe()
        {
            if (this.move_label.InvokeRequired)
                this.move_label.BeginInvoke((MethodInvoker) delegate()
                {
                    this.move_label.Text = "Move: " + myMap.moveCounter;
                });
            else
                move_label.Text = "Move: " + myMap.moveCounter;

            if (this.ai_depth_label.InvokeRequired)
                this.ai_depth_label.BeginInvoke((MethodInvoker) delegate()
                {
                    this.ai_depth_label.Text = "AI depth: " + myMap.aiDepth;
                });
            else
                ai_depth_label.Text = "AI depth: " + myMap.aiDepth;

            if (this.ai_ct_label.InvokeRequired)
                this.ai_ct_label.BeginInvoke((MethodInvoker) delegate()
                {
                    this.ai_ct_label.Text = "current time: " + currentAiTime + " ms";
                });
            else
                ai_ct_label.Text = "current time: " + currentAiTime + " ms";

            if (this.ai_lt_label.InvokeRequired)
                this.ai_lt_label.BeginInvoke((MethodInvoker) delegate()
                {
                    this.ai_lt_label.Text = "last AI time: " + aiTime + " ms";
                });
            else
                ai_lt_label.Text = "last AI time: " + aiTime + " ms";
        }

        //necessary for threading
        public void redrawPanelSafe()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(this.Refresh));
            else
                this.Refresh();
        }

        //activate / deactivate AI
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            myMap.blackAI = !myMap.blackAI;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            myMap.redAI = !myMap.redAI;
        }
    }
}
