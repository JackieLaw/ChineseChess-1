using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ChineseChess
{
    public partial class MainForm : Form
    {
        private GameController gameController;
        private Dictionary<int, Image> chessmanImagePair;
        private const int RowSum = 10;
        private const int ColSum = 9;
        private PictureBox currentChosenPictureBox = null;
        private Image currentChosenImage = null;
        private System.Timers.Timer flickerTimer;

        public MainForm()
        {
            InitializeComponent();
            //��ӱ�ź�Image��Ӧ���ֵ�
            chessmanImagePair = new Dictionary<int, Image>();
            chessmanImagePair.Add(-7, Properties.Resources.enemy7); chessmanImagePair.Add(7, Properties.Resources.friend7);
            chessmanImagePair.Add(-6, Properties.Resources.enemy6); chessmanImagePair.Add(6, Properties.Resources.friend6);
            chessmanImagePair.Add(-5, Properties.Resources.enemy5); chessmanImagePair.Add(5, Properties.Resources.friend5);
            chessmanImagePair.Add(-4, Properties.Resources.enemy4); chessmanImagePair.Add(4, Properties.Resources.friend4);
            chessmanImagePair.Add(-3, Properties.Resources.enemy3); chessmanImagePair.Add(3, Properties.Resources.friend3);
            chessmanImagePair.Add(-2, Properties.Resources.enemy2); chessmanImagePair.Add(2, Properties.Resources.friend2);
            chessmanImagePair.Add(-1, Properties.Resources.enemy1); chessmanImagePair.Add(1, Properties.Resources.friend1);
            chessmanImagePair.Add(0, null);
        }

        //�Ҽ��˵�-����Ϸ
        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //���û�����ɹ�����PictureBox����ô����10 * 9��
            if (this.panelChessman.Controls.Count == 0)
            {
                gameController = new GameController();
                for (int j = 0; j < RowSum; ++j)
                {
                    for (int i = 0; i < ColSum; ++i)
                    {
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Size = new Size(70, 70);
                        if (j < 5)
                            pictureBox.Location = new Point(i * 83, 10 + j * 83);
                        else if (j >= 5)
                            pictureBox.Location = new Point(i * 83, 15 + j * 83);
                        pictureBox.Parent = panelChessman;
                        pictureBox.BackColor = Color.Transparent;
                        pictureBox.Click += new EventHandler(ClickChess);
                        panelChessman.Controls.Add(pictureBox);
                    }
                }
                flickerTimer = new System.Timers.Timer();
                flickerTimer.Elapsed += new ElapsedEventHandler(TimerPictureBoxFlicker);
                flickerTimer.Interval = 500;
                flickerTimer.AutoReset = true;
            }
            //������Ϸ�����°ڷ�����
            this.ResetAllChessman();
            //��ʼ��������ʱ��
            flickerTimer.Enabled = false;
        }

        //������������
        private void ResetAllChessman()
        {
            gameController.Reset();
            int index = 0;
            foreach (Control control in this.panelChessman.Controls)
            {
                PictureBox pictureBox = (PictureBox)control;
                pictureBox.Image = chessmanImagePair[gameController.GetChessman(index / ColSum, index % ColSum)];
                index++;
            }
        }

        //�ƶ���������
        private bool ResetAChessman(int lasti, int lastj, int i, int j)
        {
            if (false == gameController.SetChessman(lasti, lastj, i, j))
                return false;
            PictureBox pictureBox = (PictureBox)this.panelChessman.Controls[i * ColSum + j];
            pictureBox.Image = chessmanImagePair[gameController.GetChessman(i, j)];
            PictureBox oldPictureBox = (PictureBox)this.panelChessman.Controls[lasti * ColSum + lastj];
            oldPictureBox.Image = chessmanImagePair[0];
            return true;
        }

        //�Ҽ��˵�-�����غ�
        private void SkipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameController.SkipTurn();
        }

        //�Ҽ��˵�-����
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //�Ҽ��˵�-�˳�
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //����������¼�
        private void ClickChess(object sender, EventArgs e)
        {
            //��ȡ��ǰ��PictureBox
            PictureBox pictureBox = (PictureBox)sender;
            //�ж��Ƿ��Ѿ���ѡ��
            if (currentChosenPictureBox == null && pictureBox.Image == null)
                return;
            if (currentChosenPictureBox == null)
            {
                int index = 0;
                foreach (Control control in this.panelChessman.Controls)
                {
                    if (pictureBox == (PictureBox)control)
                        break;
                    index++;
                }
                if (gameController.IsAvaliable(index / ColSum, index % ColSum))
                {
                    currentChosenPictureBox = pictureBox;
                    flickerTimer.Enabled = true;
                }
            }
            else if (currentChosenPictureBox == pictureBox)
            {
                DisableFlickerTimer();
            }
            else
            {
                //�����ѡ�У���ô�����ƶ����Ȼ�ȡ��ЩPictureBox��λ��
                PictureBox lastPictureBox = currentChosenPictureBox;
                int index = 0, thisIndex = 0, lastIndex = 0;
                foreach (Control control in this.panelChessman.Controls)
                {
                    if (lastPictureBox == (PictureBox)control)
                        lastIndex = index;
                    if (pictureBox == (PictureBox)control)
                        thisIndex = index;
                    index++;
                }
                //�����ƶ�����������ƶ�����ô�ȸı��ٽ��ã���������ƶ�����ô�Ƚ����ٸı�
                if (true == ResetAChessman(lastIndex / ColSum, lastIndex % ColSum, thisIndex / ColSum, thisIndex % ColSum))
                {
                    currentChosenPictureBox = pictureBox;
                    DisableFlickerTimer();
                }
                else
                {
                    DisableFlickerTimer();
                    currentChosenPictureBox = pictureBox;
                    flickerTimer.Enabled = true;
                }
            }
        }

        //����������ʱ����û��ѡȡ�����ӻ���ѡȡ�������Ѿ�����˱����ƶ�
        private void DisableFlickerTimer()
        {
            currentChosenPictureBox.Image = currentChosenImage;
            flickerTimer.Enabled = false;
            currentChosenImage = null;
            currentChosenPictureBox = null;
        }

        //�ؼ������¼�����Image��NULL֮�佻���Դﵽ����Ч��
        private void TimerPictureBoxFlicker(object sender, EventArgs e)
        {
            if (currentChosenPictureBox != null && currentChosenImage == null)
                currentChosenImage = currentChosenPictureBox.Image;
            if (currentChosenPictureBox.Image == null)
                currentChosenPictureBox.Image = currentChosenImage;
            else
                currentChosenPictureBox.Image = null;
        }

        //����ܼ��غ���
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.panelChessman.Parent = this.pictureBoxChessPanel;
        }
    }
}
