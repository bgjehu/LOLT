using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Media;
using System.Text.RegularExpressions;



namespace LOLT
{
    public partial class MainFrame : Form
    {

        //Boardless Form
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormBorderStyle = FormBorderStyle.None;
        }
        //Boardless Form[END]

        //Drag
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Capture = false;
                Message msg = Message.Create(this.Handle, 0XA1, new IntPtr(2), IntPtr.Zero);
                this.WndProc(ref msg);
            }
        }
        //Drag[END]

        //Variables
        private System.Drawing.Point MouseDownLocation;
        bool isStart = false;
        bool isPause = false;
        bool isMin = true;
        bool[] isInputting = new bool[6];
        int[] CampTimer = new int[6];
        Label[] CampText = new Label[6];
        TextBox[] CampBox = new TextBox[6];
        int GameTimer = 0;
        int ticks = 0;
        SoundPlayer DRAlert = new SoundPlayer(Resource00.DRin30);
        SoundPlayer BAAlert = new SoundPlayer(Resource00.BAin30);
        Regex textBoxRegex1 = new Regex(@"^\d{4}$");
        Regex textBoxRegex2 = new Regex(@"^\d{3}$");

        //Variables Mapping
        private void mapping()
        {
            CampText[0] = label1; CampText[1] = label2; CampText[2] = label3;
            CampText[3] = label4; CampText[4] = label5; CampText[5] = label6;
            CampBox[0] = textBox1; CampBox[1] = textBox2; CampBox[2] = textBox3;
            CampBox[3] = textBox4; CampBox[4] = textBox5; CampBox[5] = textBox6;
        }
        //Variable Reset
        private void variablereset()
        {
            isStart = false;
            isPause = false;
            ticks = 0;
            for (int i = 0; i < 6; i++)
            {
                isInputting[i] = false;
            }
        }


        //MainThread
        public MainFrame()
        {
            MessageBox.Show(global::LOLT.Resource00.info, "Help");
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            mapping();
            TimersInitiate();
            variablereset();
        }

        //Function
        private string formatting(int i)
        {
            string tmp = null;
            if (i > 0) { tmp = (i / 60).ToString("D2") + ":" + (i % 60).ToString("D2"); }
            else { tmp = "LIVE"; }
            return tmp;
        }
        private void TimersInitiate()
        {
            for (int i = 0; i < 6; i++) { TimerReset(i, true); }
            GameTimer = 0;
            groupBox1.Text = "00:00";     //reflect right away
        }
        private void TimerReset(int i, bool initiate)
        {
            if (initiate)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                    case 4:
                    case 5:
                        CampTimer[i] = 115;
                        CampText[i].Text = formatting(CampTimer[i]);    //reflect right away
                        break;
                    case 2:
                        CampTimer[i] = 900;
                        CampText[i].Text = formatting(CampTimer[i]);    //reflect right away
                        break;
                    case 3:
                        CampTimer[i] = 150;
                        CampText[i].Text = formatting(CampTimer[i]);    //reflect right away
                        break;
                }
            }
            else 
            {
                switch (i)
                {
                    case 0:
                    case 1:
                    case 4:
                    case 5:
                        CampTimer[i] = 300;
                        CampText[i].Text = formatting(CampTimer[i]);    //reflect right away
                        break;
                    case 2:
                        CampTimer[i] = 420;
                        CampText[i].Text = formatting(CampTimer[i]);    //reflect right away
                        break;
                    case 3:
                        CampTimer[i] = 360;
                        CampText[i].Text = formatting(CampTimer[i]);    //reflect right away
                        break;
                }
                CampBox[i].Text = formatting(GameTimer + CampTimer[i]);
            }
            
        }
        //ONE Tick
        private void OneTick()
        {
            if (CampTimer[2] == 34) { BAAlert.Play(); }
            if (CampTimer[3] == 34) { DRAlert.Play(); }
            //GameTimer
            GameTimer++;
            groupBox1.Text = formatting(GameTimer);
            //CampsTimers
            for (int i = 0; i < 6; i++) { CampTimer[i]--; }
            for (int i = 0; i < 6; i++) { CampText[i].Text = formatting(CampTimer[i]); }
            //TextBoxTime
            for (int i = 0; i < 6; i++)
            {
                if (!isInputting[i])
                {
                    CampBox[i].Text = formatting(GameTimer + CampTimer[i]);
                }
            }

        }
        //ONE Reversed Tick
        private void OneReversedTick()
        {
            if (CampTimer[2] == 34) { BAAlert.Play(); }
            if (CampTimer[3] == 34) { DRAlert.Play(); }
            //GameTimer
            GameTimer--;
            groupBox1.Text = formatting(GameTimer);
            //CampsTimers
            for (int i = 0; i < 6; i++) { CampTimer[i]++; }
            for (int i = 0; i < 6; i++) { CampText[i].Text = formatting(CampTimer[i]); }
            //TextBoxTime
            for (int i = 0; i < 6; i++)
            {
                if (!isInputting[i])
                {
                    CampBox[i].Text = formatting(GameTimer + CampTimer[i]);
                }
            }
        }

        //TextBox Input Handling
        private void TIH(int i)
        {
            if (!isStart) { CampBox[i].Text = null; isInputting[i] = false; }
            else 
            {
                int tmpnum = 0;
                if (textBoxRegex1.IsMatch(CampBox[i].Text))
                {
                    char[] tmp = CampBox[i].Text.ToCharArray(0, 4);
                    tmpnum = (int)Char.GetNumericValue(tmp[0]) * 600 + (int)Char.GetNumericValue(tmp[1]) * 60
                                 + (int)Char.GetNumericValue(tmp[2]) * 10 + (int)Char.GetNumericValue(tmp[3]);
                    if (tmpnum > GameTimer && tmpnum < 6000 && ((int)Char.GetNumericValue(tmp[2]) * 10 + (int)Char.GetNumericValue(tmp[3]))<60)
                    {
                        CampTimer[i] = tmpnum - GameTimer;
                        isInputting[i] = false;
                    }

                }
                else if (textBoxRegex2.IsMatch(CampBox[i].Text))
                {
                    char[] tmp = CampBox[i].Text.ToCharArray(0, 3);
                    tmpnum = (int)Char.GetNumericValue(tmp[0]) * 60 + (int)Char.GetNumericValue(tmp[1]) * 10
                                 + (int)Char.GetNumericValue(tmp[2]);
                    if (tmpnum > GameTimer && ((int)Char.GetNumericValue(tmp[1]) * 10 + (int)Char.GetNumericValue(tmp[2])) < 60)
                    {
                        CampTimer[i] = tmpnum - GameTimer;
                        isInputting[i] = false;
                    }
                }
                else { isInputting[i] = false; }
            }
        }


        //Event
        //Drag Button
        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }

        private void button10_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Left = e.X + this.Left - MouseDownLocation.X;
                this.Top = e.Y + this.Top - MouseDownLocation.Y;
            }
        }
        //Tick Event
        private void timer1_Tick(object sender, EventArgs e)
        {
            ticks++;
            if (ticks % 90 != 0) { OneTick(); }
        }
        //Drag Button[END]

        //Start Button_Click
        private void button7_Click(object sender, EventArgs e)
        {
            if (!isStart)       //Have NOT started
            {
                timer1.Start();
                isStart = true;
                button7.BackgroundImage = global::LOLT.Resource00.PAUSE;
            }
            else 
            {
                if (!isPause)       //Started but it is NOT paused
                {
                    timer1.Stop();
                    isPause = true;
                    button7.BackgroundImage = global::LOLT.Resource00.START;
                }
                else        //Started and it is paused
                {
                    timer1.Start();
                    isPause = false;
                    button7.BackgroundImage = global::LOLT.Resource00.PAUSE;
                }
            }
        }
        //Start Button_Click[END]

        private void button11_Click(object sender, EventArgs e)
        {
            if (!isStart) { }
            else 
            {
                timer1.Stop();
                isPause = true;
                isStart = false;
                button7.BackgroundImage = global::LOLT.Resource00.START;
                TimersInitiate();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int j = 0;
            while (j < 5)
            {
                OneTick();
                j++;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int j = 0;
            while (j < 5)
            {
                OneReversedTick();
                j++;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TimerReset(0, false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TimerReset(1, false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TimerReset(2, false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TimerReset(3, false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TimerReset(4, false);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TimerReset(5, false);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            isInputting[0] = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            isInputting[1] = true;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            isInputting[2] = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            isInputting[3] = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            isInputting[4] = true;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            isInputting[5] = true;
        }
        void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                TIH(0);   
            }
        }
        void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                TIH(1);
            }
        }
        void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                TIH(2);
            }
        }
        void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                TIH(3);
            }
        }
        void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                TIH(4);
            }
        }
        void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                TIH(5);
            }
        }
        void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (!isStart) { textBox7.Text = null; }
                else
                {
                    int tmpnum = 0;
                    int diff = 0;
                    if (textBoxRegex1.IsMatch(textBox7.Text))
                    {
                        char[] tmp = textBox7.Text.ToCharArray(0, 4);
                        tmpnum = (int)Char.GetNumericValue(tmp[0]) * 600 + (int)Char.GetNumericValue(tmp[1]) * 60
                                     + (int)Char.GetNumericValue(tmp[2]) * 10 + (int)Char.GetNumericValue(tmp[3]);
                        if (tmpnum < 6000 && ((int)Char.GetNumericValue(tmp[2]) * 10 + (int)Char.GetNumericValue(tmp[3])) < 60)
                        {
                            diff = tmpnum - GameTimer;
                            GameTimer = tmpnum;
                            for (int i = 0; i < 6; i++)
                            {
                                CampTimer[i] -= diff;
                                CampText[i].Text = formatting(CampTimer[i]);
                            }
                        }

                    }
                    else if (textBoxRegex2.IsMatch(textBox7.Text))
                    {
                        char[] tmp = textBox7.Text.ToCharArray(0, 3);
                        tmpnum = (int)Char.GetNumericValue(tmp[0]) * 60 + (int)Char.GetNumericValue(tmp[1]) * 10
                                     + (int)Char.GetNumericValue(tmp[2]);
                        if (((int)Char.GetNumericValue(tmp[1]) * 10 + (int)Char.GetNumericValue(tmp[2])) < 60)
                        {
                            diff = tmpnum - GameTimer;
                            GameTimer = tmpnum;
                            for (int i = 0; i < 6; i++)
                            {
                                CampTimer[i] -= diff;
                                CampText[i].Text = formatting(CampTimer[i]);
                            }
                        }
                    }
                    textBox7.Text = null;
                    groupBox1.Text = formatting(GameTimer);
                }
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(global::LOLT.Resource00.info, "Help");
        }


        
        
        

       
        
        
        

        

    }
}
