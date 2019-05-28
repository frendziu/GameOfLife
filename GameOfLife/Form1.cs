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

namespace GameOfLife
{
    public partial class Form1 : Form
    {

        private Bitmap bitmap;
        private Graphics graphics;
        private int vertical_cells_num;
        private int horizontal_cells_num;
        private int cell_size_value;
        private bool[,] cells_status;
        private string pattern;
        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);

            Patterns();
            Change_grid_size();
            Grid_size();
        }

        private void Grid_size()
        {
            horizontal_cells_num = pictureBox1.Height / cell_size_value;
            vertical_cells_num = pictureBox1.Width / cell_size_value;

            cells_status = new bool[horizontal_cells_num, vertical_cells_num];

            Match_Pattern();

            PrintGrid();
        }

        private void Match_Pattern()
        {
            switch (pattern)
            {
                case "Empty":
                    {
                        break;
                    }
               
                case "Niezmienne":
                    {
                        cells_status[horizontal_cells_num / 2 - 1, vertical_cells_num / 2] = true;
                        cells_status[horizontal_cells_num / 2 - 1, vertical_cells_num / 2 + 1] = true;
                        cells_status[horizontal_cells_num / 2 , vertical_cells_num / 2 - 1] = true;
                        cells_status[horizontal_cells_num / 2 , vertical_cells_num / 2 + 2] = true;
                        cells_status[horizontal_cells_num / 2 + 1, vertical_cells_num / 2] = true;
                        cells_status[horizontal_cells_num / 2 + 1, vertical_cells_num / 2 + 1] = true;

                        break;
                    }

                case "Oscylator":
                    {
                        cells_status[horizontal_cells_num / 2 - 1, vertical_cells_num / 2] = true;
                        cells_status[horizontal_cells_num / 2, vertical_cells_num / 2] = true;
                        cells_status[horizontal_cells_num / 2 + 1, vertical_cells_num / 2] = true;
                        break;
                    }
                
                case "Glider":
                    {
                        cells_status[horizontal_cells_num / 2, vertical_cells_num / 2] = true;
                        cells_status[horizontal_cells_num / 2 - 1, vertical_cells_num / 2 - 1] = true;
                        cells_status[horizontal_cells_num / 2, vertical_cells_num / 2 + 1] = true;
                        cells_status[horizontal_cells_num / 2 + 1, vertical_cells_num / 2] = true;
                        cells_status[horizontal_cells_num / 2 + 1, vertical_cells_num / 2 - 1] = true;

                        break;
                    }
 
                case "Random":
                    {
                        Random rand = new Random();
                        for (int i = 0; i < horizontal_cells_num; i++)
                        {
                            for (int j = 0; j < vertical_cells_num; j++)
                            {
                                cells_status[i, j] = rand.NextDouble() >= 0.6;
                            }
                        }

                        break;
                    }
            }
        }

        private void Change_grid_size()
        {
            numericUpDown1.Minimum = 2;
            numericUpDown1.Maximum = 100;
            numericUpDown1.Value = 50;

        }

        private void Patterns()
        {
            comboBox1.Items.Add("Empty");            
            comboBox1.Items.Add("Niezmienne");
            comboBox1.Items.Add("Oscylator");           
            comboBox1.Items.Add("Glider");
            comboBox1.Items.Add("Random"); 
        }

        private void Start_Game_iterations()
        {

            while (checkBox1.Checked)
            {

                Rules();
                PrintGrid();
                Thread.Sleep(1000);
            }

        }

        private void Start_Game_step_by_step()
        {
            Rules();
            PrintGrid();
            Thread.Sleep(1000);

        }

        private void Rules()
        {
            bool[,] sasiednia_status = new bool[horizontal_cells_num, vertical_cells_num];

            for (int i = 0; i < horizontal_cells_num; i++)
            {
                for (int j = 0; j < vertical_cells_num; j++)
                {
                    int sasiedzi = Count_Neighbours(i, j);
                    bool zywa = cells_status[i, j];

                    if (zywa)
                    {
                        //umiera
                        if (sasiedzi < 2)
                        {
                            sasiednia_status[i, j] = false;
                        }
                        else if (sasiedzi > 3)
                        {
                            sasiednia_status[i, j] = false;
                        }
                        else
                        {
                            sasiednia_status[i, j] = true;
                        }
                    }
                    else
                    {
                       //staje sie zywa
                        if (sasiedzi == 3)
                        {
                            sasiednia_status[i, j] = true;
                        }
                        else
                        {
                            sasiednia_status[i, j] = false;
                        }
                    }
                }
            }

            cells_status = sasiednia_status;
        }

        private int Count_Neighbours(int x, int y)
        {
            int count = 0;

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }

                    int index_x = i;
                    int index_y = j;

                    if (i == -1)
                    {
                        index_x = horizontal_cells_num - 1;
                    }
                    else if (i == horizontal_cells_num)
                    {
                        index_x = 0;
                    }

                    if (j == -1)
                    {
                        index_y = vertical_cells_num - 1;
                    }
                    else if (j == vertical_cells_num)
                    {
                        index_y = 0;
                    }

                    if (cells_status[index_x, index_y])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void PrintGrid()
        {
            lock (graphics)
            {
                graphics.Clear(Color.White);
                Pen black_pen = new Pen(Color.Black);
                Brush black_brush = new SolidBrush(Color.DarkGray);

                for (int i = 0; i < horizontal_cells_num + 1; i++)
                {
                    graphics.DrawLine(black_pen, new Point(0, i * cell_size_value), new Point(vertical_cells_num * cell_size_value, i * cell_size_value));
                }

                for (int i = 0; i < vertical_cells_num + 1; i++)
                {
                    graphics.DrawLine(black_pen, new Point(i * cell_size_value, 0), new Point(i * cell_size_value, horizontal_cells_num * cell_size_value));
                }

                for (int i = 0; i < horizontal_cells_num; i++)
                {
                    for (int j = 0; j < vertical_cells_num; j++)
                    {
                        if (cells_status[i, j])
                        {
                            Rectangle rectangle = new Rectangle(
                                j * cell_size_value,
                                i * cell_size_value,
                                cell_size_value,
                                cell_size_value
                            );

                            graphics.FillRectangle(black_brush, rectangle);
                        }
                    }
                }

                pictureBox1.Image = bitmap;
            }
        }







        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;

            int i = coordinates.Y / cell_size_value;
            int j = coordinates.X / cell_size_value;

            bool status = cells_status[i, j];

            if (status)
            {
                cells_status[i, j] = false;
            }
            else
            {
                cells_status[i, j] = true;
            }

            PrintGrid();
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            pattern = comboBox1.SelectedItem.ToString();
            Grid_size();
        }



        private void Button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Thread thr = new Thread(Start_Game_iterations);
                thr.IsBackground = true;
                thr.Start();
            }

        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            cell_size_value = decimal.ToInt32(numericUpDown1.Value);
            Grid_size();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void Button2_Click(object sender, EventArgs e)
        {

            Thread thr = new Thread(Start_Game_step_by_step);
            thr.IsBackground = true;
            thr.Start();

        }
    }
}
