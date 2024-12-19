using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarRacing
{
    public partial class pictBoxStartLine : Form
    {
        #region zmienne

        List<PictureBox> listLines;
        List<PictureBox> listEnemies;
        List<PictureBox> listCoins;

        int speed = 0;
        int coins = 0;
        int distance = 0;

        bool isGameOver = false;
        bool isStart = true;
        bool isFinish = false;
        bool isPause = false;
        PictureBox countedCoin;

        //startcounting
        int counter = 3;

        #endregion

        public pictBoxStartLine()
        {
            InitializeComponent();

            listLines = new List<PictureBox>() { pictBoxLine1, pictBoxLine2, pictBoxLine3, pictBoxLine4 };
            listEnemies = new List<PictureBox>() { pictBoxEnemy1, pictBoxEnemy2, pictBoxEnemy3 };
            listCoins = new List<PictureBox>() { pictBoxCoin1, pictBoxCoin2 };
        }

        #region methods

        private void moveLines(int speed)
        {
            foreach(PictureBox p in listLines)
            {
                if(p.Top < 400)
                {
                    p.Top += speed;
                }
                else
                {
                    p.Top = 0;
                }
            }

            pictBoxStart.Top += speed;
            distance += speed;

            if(distance >= 15000)
            {
                pictBoxMeta.Visible = true;
                pictBoxMeta.Top += speed;
            }
        }

        private void enemiesMove(int speed)
        {
            Random rand = new Random();
            bool collision;

            foreach(PictureBox p in listEnemies)
            {
                if(p.Top < 400)
                {
                    p.Top += speed / 2;
                }
                else
                {
                    do
                    {
                        collision = false;
                        p.Location = new Point(rand.Next(44, 310), 0);

                        foreach (PictureBox p2 in listCoins)
                        {
                            if (p2.Bounds.IntersectsWith(p.Bounds))
                            {
                                collision = true;
                            }
                        }
                    } while (collision);

                }
            }
        }

        private void GameOver()
        {
            foreach(PictureBox p in listEnemies)
            {
                if(p.Bounds.IntersectsWith(pictBoxCar.Bounds))
                {
                    panelGameOver.Visible = true;
                    speed = 0;
                    isGameOver = true;
                    btnStart.Visible = true;

                    break;
                }
            }
        }

        private void CoinsMove()
        {
            bool collision;
            Random rand = new Random();

            foreach(PictureBox p in listCoins)
            {
                if(p.Top < 400)
                {
                    p.Top += speed / 2;
                }
                else
                {
                    p.Visible = true;

                    if(p == countedCoin)
                    {
                        countedCoin = null;
                    }

                    do
                    {
                        collision = false;
                        p.Location = new Point(rand.Next(34, 320), 0);

                        //monety nie mogą się zazębiać z wrogimi autami
                        foreach (PictureBox p2 in listEnemies)
                        {
                            if (p2.Bounds.IntersectsWith(p.Bounds))
                            {
                                collision = true;
                            }
                        }

                    } while (collision);

                }
            }
        }

        private void CoinPick()
        {
            foreach(PictureBox p in listCoins)
            {
                if(p.Bounds.IntersectsWith(pictBoxCar.Bounds) && p != countedCoin)
                {
                    countedCoin = p;
                    coins++;
                    lblCoins.Text = "Coins: " + coins;
                    p.Visible = false;
                }
            }
        }

        private void StartCounting()
        {
            timerMove.Enabled = false;
            speed = 4;
            coins = 0;
            distance = 0;
            isStart = true;

            if(counter >= 0)
            {
                lblCounting.Text = counter.ToString();
                counter--;
            }
            else
            {
                timerStart.Enabled = false;
                panelStart.Visible = false;
                panelGameOver.Visible = false;
                isStart = false;
                timerMove.Enabled = true;
            }
        }

        private void Finish()
        {
            if(pictBoxCar.Bounds.IntersectsWith(pictBoxMeta.Bounds))
            {
                timerMove.Enabled = false;
                isFinish = true;
                panelGameOver.Visible = true;
                panelStart.Visible = true;
                panelFinish.Visible = true;
                btnStart.Visible = true;
            }
        }

        #endregion

        #region timers

        private void timerMove_Tick(object sender, EventArgs e)
        {
            moveLines(speed);

            enemiesMove(speed);

            GameOver();

            CoinsMove();

            CoinPick();

            Finish();
        }

        private void timerStart_Tick(object sender, EventArgs e)
        {
            StartCounting();
        }

        #endregion

        #region events

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && !isGameOver && !isStart && !isFinish && !isPause && speed > 0)
            {
                if(pictBoxCar.Left >= 49)
                {
                    pictBoxCar.Left -= 15;
                }
            }
            else if (e.KeyCode == Keys.Right && !isGameOver && !isStart && !isFinish && !isPause && speed > 0)
            {
                if(pictBoxCar.Right <= 339)
                {
                    pictBoxCar.Left += 15;
                }
            }
            else if (e.KeyCode == Keys.Up && !isGameOver && !isStart && !isFinish && !isPause)
            {
                if (speed < 10)
                {
                    speed += 2;
                }
            }
            else if (e.KeyCode == Keys.Down && !isGameOver && !isStart && !isFinish && !isPause)
            {
                if (speed > 0)
                {
                    speed -= 2;
                }
            }
            else if(e.KeyCode == Keys.Space && !isGameOver && !isStart && !isFinish && !isPause)
            {
                timerMove.Enabled = false;
                isPause = true;

                panelGameOver.Visible = true;
                panelStart.Visible = true;
                panelFinish.Visible = true;
                panelPasue.Visible = true;
            }
            //Odpauzowanie
            else if(e.KeyCode == Keys.Space && !isGameOver && !isStart && !isFinish && isPause)
            {
                timerMove.Enabled = true;
                isPause = false;

                panelGameOver.Visible = false;
                panelStart.Visible = false;
                panelFinish.Visible = false;
                panelPasue.Visible = false;
            }
            
        }

        #endregion

        private void btnStart_Click(object sender, EventArgs e)
        {
            isGameOver = false;
            isFinish = false;

            pictBoxStart.Location = new Point(25, 251);
            pictBoxEnemy1.Location = new Point(290, 99);
            pictBoxEnemy2.Location = new Point(131, -1);
            pictBoxEnemy3.Location = new Point(230, 202);
            pictBoxCoin1.Location = new Point(277, 12);
            pictBoxCoin2.Location = new Point(39, 139);
            pictBoxCoin1.Visible = true;
            pictBoxCoin2.Visible = true;
            pictBoxCar.Location = new Point(85, 258);
            btnStart.Visible = false;
            panelFinish.Visible = false;
            panelGameOver.Visible = false;
            panelStart.Visible = true;

            counter = 3;
            coins = 0;
            lblCoins.Text = "Coins: " + coins;

            timerMove.Enabled = false;
            timerStart.Enabled = true;

            pictBoxMeta.Top = -1;
            pictBoxMeta.Visible = false;

            StartCounting();
        }
    }
}
