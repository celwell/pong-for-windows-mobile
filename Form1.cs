using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;

namespace Pong
{
	/// <summary>
	/// 
	/// Ideas:
	/// add spin abilities to be SpinPong
	/// 
	/// </summary>
	/// 

	public class Form1 : System.Windows.Forms.Form
	{
		// screen width and height
		private int scrWidth=320, scrHeight=210;

		private double ballPosX=-1, ballPosY=-1, ballVelX=-1, ballVelY=-1;
		// 25 is default tail length
		//private double[] ballPrevX = new double[25];
		//private double[] ballPrevY = new double[25];


		// even numbers only (due to some (pSize/2) in the code)
		private int pSize = 28;
		private double p1 = 105;
		private double p2 = 105;
		private double p1v = 0;
		private double p2v = 0;

		// 3 = default; pSpeed is the velocity when holding down a key; maximum velocity of a paddle
		private double pSpeed = 3.2;
	
		private int p1score = 0;
		private int p2score = -1;
		
		private bool W_DOWN = false;
		private bool S_DOWN = false;
		private bool UPARROW_DOWN = false;
		private bool DOWNARROW_DOWN = false;
	
		private bool paused = true;
		private bool infoBox = true;

		private int difficulty = 1;
		private string[] difficultyNames = {"human","easy","medium","hard"};
	
		private int[] upArrowX = {436,446,436};
		private int[] upArrowY = {354,364,374};
		private int[] downArrowX = {355,345,355};
		private int[] downArrowY = {354,364,374};

		private Random r = new Random();

		Bitmap offScreenBmp;
		Graphics offScreenDC;

		private Timer t = new Timer();

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			offScreenBmp = new Bitmap(scrWidth, scrWidth);
			offScreenDC = Graphics.FromImage(offScreenBmp);
			t.Interval = 1;
			t.Enabled = true;
			t.Tick += new EventHandler(t_Tick);
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Form1
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(314, 183);
			this.Text = "Pong - by Chris Elwell";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		static void Main() 
		{
			Application.Run(new Form1());
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{

		}

		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			//offScreenDC.FillRectangle(new SolidBrush(Color.Black), 0,	0, 320, 215);
			offScreenDC.Clear(Color.Black);
			offScreenDC.DrawRectangle(new Pen(Color.White),0,0,319,210);
			offScreenDC.FillRectangle(new SolidBrush(Color.White), 5, (int)p1-pSize/2, 5, pSize);
			offScreenDC.FillRectangle(new SolidBrush(Color.White), 310, (int)p2-pSize/2, 5, pSize);
			offScreenDC.FillEllipse(new SolidBrush(Color.White), (int)ballPosX-3, (int)ballPosY-3, 6, 6);
			// draw scores
			offScreenDC.DrawString(""+p1score, new Font("Arial", 10, System.Drawing.FontStyle.Bold), new SolidBrush(Color.White), 60, 7);
			offScreenDC.DrawString(""+p2score, new Font("Arial", 10, System.Drawing.FontStyle.Bold), new SolidBrush(Color.White), 260, 7);
			if (infoBox)
			{
				offScreenDC.DrawString("P O N G", new Font("Arial", 10, System.Drawing.FontStyle.Bold), new SolidBrush(Color.White), 130, 80);
				offScreenDC.DrawString(difficultyNames[difficulty], new Font("Arial", 10, System.Drawing.FontStyle.Bold), new SolidBrush(Color.White), 153-4*difficultyNames[difficulty].Length, 120);
			}
			else
			{
				offScreenDC.DrawString("press * to end game", new Font("Arial", 10, System.Drawing.FontStyle.Bold), new SolidBrush(Color.White), 92, 186);
			}
			g.DrawImage(offScreenBmp, 0, 0);
		}

		private void t_Tick(object sender, EventArgs e)
		{
			if (ballPosX < 0)
			{
				p2score++;
				ballPosX = 10;
				ballPosY = p1;
				//for (int i=0; i<ballPrevX.Length; i++)
				//{
				//	ballPrevX[i] = ballPosX;
				//	ballPrevY[i] = ballPosY;
				//}
				double angle = r.NextDouble()*(Math.PI/2)-(Math.PI/4);
				ballVelX = Math.Cos(angle)*4.0;
				ballVelY = Math.Sin(angle)*4.0;
				paused = true;
			}
			else if (ballPosX > 320)
			{
				p1score++;
				ballPosX = 310;
				ballPosY = p2;
				//for (int i=0; i<ballPrevX.Length; i++)
				//{
				//	ballPrevX[i] = ballPosX;
				//	ballPrevY[i] = ballPosY;
				//}
				double angle = r.NextDouble()*(Math.PI/2)+(3*Math.PI/4);
				ballVelX = Math.Cos(angle)*4.0;
				ballVelY = Math.Sin(angle)*4.0;
				paused = true;
			}
		
			if (UPARROW_DOWN)
				p2v = -pSpeed;
			else if (DOWNARROW_DOWN)
				p2v = pSpeed;
		
			if (difficulty != 0)
			{
				int skill=20;
				if (difficulty == 1)
					skill = 27;
				else if (difficulty == 2)
					skill = 17;
				else if (difficulty == 3)
					skill = 8;
			
				if (ballPosX < (240+skill*ballVelX) && Math.Abs(ballPosY-p1) > pSize/2-3)
				{
					if (ballPosY < p1 )
						p1v = -pSpeed;
					else 
						p1v = pSpeed;
				}
				if (paused)
				{
					if (!(p1 > 95 && p1 < 115))
					{
						if (p1 > 105)
							p1v = -pSpeed;
						if (p1 < 105)
							p1v = pSpeed;
					}
				}
			}
			else
			{
				if (W_DOWN)
					p1v = -pSpeed;
				else if (S_DOWN)
					p1v = pSpeed;
			}
		
			p1 += p1v;
			p2 += p2v;
			// .95 = default; friction
			p1v *= .70;
			p2v *= .70;
		
			if (!paused)
			{
				ballPosX += ballVelX;
				ballPosY += ballVelY;
			}
			else
				ballPosY = (ballPosX < 160) ? p1 : p2;
		
			if (p1 < pSize/2)
				p1 = pSize/2;
			else if (p1 > scrHeight-pSize/2)
				p1 = scrHeight-pSize/2;
			if (p2 < pSize/2)
				p2 = pSize/2;
			else if (p2 > scrHeight-pSize/2)
				p2 = scrHeight-pSize/2;
				
			if (ballPosX < 10 && ballPosX > 1 && ballPosY > p1-pSize/2 && ballPosY < p1+pSize/2)
			{
				ballPosX = 8;
				double angle = ((ballPosY-p1)/pSize)*Math.PI/5;
				double tVel = Math.Sqrt(ballVelX*ballVelX+ballVelY*ballVelY) + Math.Abs(ballPosY-p1)*.05; // .02 = default
				ballVelX = Math.Cos(angle)*tVel;
				ballVelY = Math.Sin(angle)*tVel;
				// collisionSoundLow.play();
			}
			else if (ballPosX > 310 && ballPosX < 319 && ballPosY > p2-pSize/2 && ballPosY < p2+pSize/2)
			{
				ballPosX = 310;
				double angle = Math.PI-((ballPosY-p2)/pSize)*Math.PI/5;
				double tVel = Math.Sqrt(ballVelX*ballVelX+ballVelY*ballVelY) + Math.Abs(ballPosY-p2)*.05; // .02 = default
				ballVelX = Math.Cos(angle)*tVel;
				ballVelY = Math.Sin(angle)*tVel;
				// collisionSoundHigh.play();
			}
			if (ballPosY < 3)
			{
				ballPosY = 3;
				ballVelY = Math.Abs(ballVelY);
				// collisionSoundWall.play();
			}
			else if (ballPosY > 207)
			{
				ballPosY = 207;
				ballVelY = -Math.Abs(ballVelY);
				// collisionSoundWall.play();
			}

			Invalidate();
		}

		private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
		
			if (e.KeyCode.ToString() == "Up" || e.KeyCode.ToString() == "D3")
				UPARROW_DOWN = true;
			else if (e.KeyCode.ToString() == "Down" || e.KeyCode.ToString() == "D9")
				DOWNARROW_DOWN = true;
			else if (e.KeyCode.ToString() == "W")
				W_DOWN = true;
			else if (e.KeyCode.ToString() == "Z")
				S_DOWN = true;
			else if (e.KeyCode.ToString() == "Left")
			{
				if (infoBox)
				{
					difficulty--;
					if (difficulty < 0)
						difficulty = difficultyNames.Length-1;
				}
			}
			else if (e.KeyCode.ToString() == "Right")
			{
				if (infoBox)
				{
					difficulty++;
					if (difficulty > difficultyNames.Length-1)
						difficulty = 0;
				}
			}			
			else if (e.KeyCode.ToString() == "D0")
			{
				infoBox = false;
				paused = false;
			}
			else if (e.KeyCode.ToString() == "Return")
			{
				infoBox = false;
				paused = false;
			}
			else if (e.KeyCode.ToString() == "F8")
			{
				p1score = 0;
				p2score = 0;
				paused = true;
				infoBox = true;
				ballPosX = 7;
				ballPosY = p1;
				double angle = r.NextDouble()*(Math.PI/2)-(Math.PI/4);
				ballVelX = Math.Cos(angle)*4.0;
				ballVelY = Math.Sin(angle)*4.0;
			}
			else if (e.KeyCode.ToString() == "F9")
				Application.Exit();
		}

		private void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode.ToString() == "Up" || e.KeyCode.ToString() == "D3")
				UPARROW_DOWN = false;
			else if (e.KeyCode.ToString() == "Down" || e.KeyCode.ToString() == "D9")
				DOWNARROW_DOWN = false;
			else if (e.KeyCode.ToString() == "W")
				W_DOWN = false;
			else if (e.KeyCode.ToString() == "Z")
				S_DOWN = false;
		}
	}
}
