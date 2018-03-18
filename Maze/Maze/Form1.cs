using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Maze
{
    public partial class Form1 : Form
    {
        const int height = 21;
        const int width = 23;
        int size = 15;// size of cell

        Point start = new Point(17, 21);
        Point finish = new Point(height - 8, width - 2);

        PictureBox[,] cell = new PictureBox[height, width];
        Button btnBuildMaze = new Button();
        Button btnGoMaze = new Button();

        public Form1()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(height * size + 55, width * size + 105);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnBuildMaze.Location = new Point(20, width * size + 30);
            btnBuildMaze.Text = "Build Maze";
            btnBuildMaze.Click += new EventHandler(btnBuildMazeClick);
            this.Controls.Add(btnBuildMaze);

            btnGoMaze.Location = new Point(20 + btnBuildMaze.Width + size, width * size + 30);
            btnGoMaze.Text = "Go Maze";
            btnGoMaze.Click += new EventHandler(btnGoClick);
            this.Controls.Add(btnGoMaze);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cell[i, j] = new PictureBox();
                    cell[i, j].Size = new System.Drawing.Size(size, size);
                    cell[i, j].Location = new Point(i * size + 20, j * size + 20);
                    this.Controls.Add(cell[i, j]);
                }
            }

            CoordAndStatus[,] c = GenerateMaze();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cell[i, j].Tag = null;
                    cell[i, j].Tag = c[i, j];
                    cell[i, j].BackColor = (c[i, j].visit)?Color.Yellow:Color.Black;
                }
            }

            cell[start.X, start.Y].BackColor = Color.Green;
            cell[finish.X, finish.Y].BackColor = Color.Red;
        }

        private void btnGoClick(object sender, EventArgs e)
        {
            this.SuspendLayout();
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    CoordAndStatus c = (CoordAndStatus)cell[i, j].Tag;
                    if (c.visit)
                    {
                        c.visit = false;
                        cell[i, j].Tag = null;
                        cell[i, j].Tag = c;
                    }
                }

            CoordAndStatus[,] cc = FindWay();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cell[i, j].Tag = null;
                    cell[i, j].Tag = cc[i, j];
                    if (cc[i, j].visit)
                        cell[i, j].BackColor = Color.Violet;
                }
            }
            cell[start.X, start.Y].BackColor = Color.Green;
            cell[finish.X, finish.Y].BackColor = Color.Red;
            this.ResumeLayout();
        }

        public struct Way
        {
            Point point { get; set; }
            bool way { get; set; }
        }

        private CoordAndStatus[,] FindWay()
        {
            CoordAndStatus[,] cells = GetTag();
            List<Way> way = new List<Way>();

            CoordAndStatus startCell = cells[start.X, start.Y];
            startCell.visit = true;
            CoordAndStatus currentCell = startCell;

            Stack stack = new Stack();

            List<CoordAndStatus> notVisitNeibourth;

            while (currentCell.x != finish.X || currentCell.y != finish.Y)
            {
                notVisitNeibourth = getNotVisitNeighbourthInMaze(currentCell, cells);

                if (notVisitNeibourth.Count > 0)
                {
                    stack.Push(currentCell);
                    Random rand = new Random();
                    int n = (notVisitNeibourth.Count == 1) ? 0 : rand.Next(notVisitNeibourth.Count, 2452) % notVisitNeibourth.Count;
                    CoordAndStatus neib = cells[notVisitNeibourth[n].x, notVisitNeibourth[n].y];
                    currentCell = neib;
                    currentCell.visit = true;
                }
                else if (stack.Count > 0)
                {
                    currentCell = (CoordAndStatus)stack.Pop();

                }
                else
                {
                    MessageBox.Show("Fail!!!");
                }
            }

            return cells;
        }

        private CoordAndStatus[,] GenerateMaze()
        {
            CoordAndStatus[,] cells = new CoordAndStatus[height,width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    cells[i, j] = new CoordAndStatus(i, j, (i % 2 == 1 && j % 2 == 1 && i != 0 && j != 0 && i != height - 1 && j != width - 1) ? false : true, false);

            CoordAndStatus startCell = cells[start.X, start.Y];
            startCell.visit = true;
            CoordAndStatus currentCell = startCell;

            Stack stack = new Stack();
            do
            {
                List<CoordAndStatus> notVisitNeibourth = getNeighborth(currentCell, cells);

                if (notVisitNeibourth.Count > 0)
                {
                    stack.Push(currentCell);
                    Random rand = new Random();
                    int n = (notVisitNeibourth.Count==1) ? 0 : rand.Next(notVisitNeibourth.Count, 24513) % notVisitNeibourth.Count;
                    CoordAndStatus neib = cells[notVisitNeibourth[n].x, notVisitNeibourth[n].y];
                    removeWall(currentCell, neib, cells);
                    currentCell = neib;
                    currentCell.visit = true;
                }
                else if (stack.Count > 0)
                {
                    currentCell = (CoordAndStatus)stack.Pop();
                }
            }
            while (GetAllNotVisitedCells(cells) > 0);
            return cells;
        }

        private void btnBuildMazeClick(object sender, EventArgs e)
        {
            this.SuspendLayout();
            CoordAndStatus[,] c = GenerateMaze();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cell[i, j].Tag = null;
                    cell[i, j].Tag = c[i, j];
                    cell[i, j].BackColor = (c[i, j].visit)?Color.Yellow: Color.Black;
                }
            }
            this.ResumeLayout();
        }

        private int GetAllNotVisitedCells(CoordAndStatus[,] cell)
        {
            List<CoordAndStatus> lnvc = new List<CoordAndStatus>();
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (!cell[i, j].visit && !cell[i, j].wall)
                        lnvc.Add(cell[i, j]);
            return lnvc.Count;
        }

        private List<CoordAndStatus> getNeighborth(CoordAndStatus currentCell, CoordAndStatus[,] cells)
        {
            List<CoordAndStatus> neighbourth = new List<CoordAndStatus>();
            Point[] p = new Point[4];
            p[0] = new Point(currentCell.x + 2, currentCell.y);
            p[1] = new Point(currentCell.x - 2, currentCell.y);
            p[2] = new Point(currentCell.x, currentCell.y + 2);
            p[3] = new Point(currentCell.x, currentCell.y - 2);

            for (int i = 0; i < p.Length; i++)
            {
                try
                {
                    if (!cells[p[i].X, p[i].Y].visit)
                        neighbourth.Add(cells[p[i].X, p[i].Y]);
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            return neighbourth;
        }
       
        private Point Bettwen(CoordAndStatus neighbourCell, CoordAndStatus currentCell)
        {
            int xDiff = neighbourCell.x - currentCell.x;
            int yDiff = neighbourCell.y - currentCell.y;

            xDiff = (xDiff != 0) ? (xDiff / Math.Abs(xDiff)) : 0;
            yDiff = (yDiff != 0) ? (yDiff / Math.Abs(yDiff)) : 0;

            Point target = new Point();

            target.X = currentCell.x + xDiff; //координаты стенки
            target.Y = currentCell.y + yDiff;

            return target;
        }

        private List<CoordAndStatus> getNotVisitNeighbourthInMaze(CoordAndStatus currentCell, CoordAndStatus[,] cells)
        {
            List<CoordAndStatus> neighbourth = new List<CoordAndStatus>();
            Point[] p = new Point[4];
            p[0] = new Point(currentCell.x + 2, currentCell.y);
            p[1] = new Point(currentCell.x - 2, currentCell.y);
            p[2] = new Point(currentCell.x, currentCell.y + 2);
            p[3] = new Point(currentCell.x, currentCell.y - 2);

            for (int i = 0; i < p.Length; i++)
            {
                try
                {
                    if (!cells[p[i].X, p[i].Y].visit)
                    {
                        Point b = Bettwen(currentCell, cells[p[i].X, p[i].Y]);
                        if (!cells[b.X, b.Y].wall)
                        {
                            cells[b.X, b.Y].visit = true;
                            neighbourth.Add(cells[p[i].X, p[i].Y]);
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            return neighbourth;
        }

        private void removeWall(CoordAndStatus currentCell, CoordAndStatus neighbourCell, CoordAndStatus[,] cells)
        {
            Point p = Bettwen(currentCell,neighbourCell);

            cells[p.X, p.Y].wall = false;
            cells[p.X, p.Y].visit = true;
        }

        private CoordAndStatus[,] GetTag()
        {
            CoordAndStatus[,] s = new CoordAndStatus[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    s[i, j] = (CoordAndStatus)this.cell[i, j].Tag;
            return s;
        }

    }

    public class CoordAndStatus
    {
        public int x { set; get; }
        public int y { set; get; }
        public bool wall { set; get; }
        public bool visit { set; get; }

        public CoordAndStatus(int _x, int _y, bool _wall, bool _v)
        {
            x = _x;
            y = _y;
            wall = _wall;
            visit = _v;
        }

        public CoordAndStatus(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public CoordAndStatus(bool _v)// if it's wall true else false
        {
            visit = _v;
        }

        public CoordAndStatus(bool _wall, bool _visit)
        {
            wall = _wall;
            visit = _visit;
        }
    }
}
