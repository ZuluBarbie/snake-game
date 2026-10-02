using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public sealed class SnakeWindow : Form
    {
        const int Columns = 28, Rows = 22, Cell = 22, LeftEdge = 24, TopEdge = 100;
        readonly List<Point> snake = new List<Point>();
        readonly Random random = new Random();
        readonly Timer timer = new Timer();
        Point food, direction, nextDirection;
        bool ended, paused, turned, won;
        int score, best;

        public SnakeWindow()
        {
            Text = "Snake | C# Arcade";
            ClientSize = new Size(Columns * Cell + 48, Rows * Cell + 158);
            BackColor = Color.FromArgb(14, 20, 30);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 11);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            KeyPreview = true;
            timer.Tick += delegate { Step(); };
            KeyDown += HandleKey;
            Deactivate += delegate { if (!ended) { paused = true; Invalidate(); } };
            Reset();
        }

        void Reset()
        {
            snake.Clear();
            for (int x = 10; x >= 6; x--) snake.Add(new Point(x, 11));
            direction = nextDirection = new Point(1, 0);
            score = 0;
            ended = paused = turned = won = false;
            PlaceFood();
            timer.Interval = 135;
            timer.Start();
            Invalidate();
        }

        void PlaceFood()
        {
            var free = new List<Point>();
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                {
                    Point point = new Point(x, y);
                    if (!snake.Contains(point)) free.Add(point);
                }
            if (free.Count == 0) { won = ended = true; timer.Stop(); return; }
            food = free[random.Next(free.Count)];
        }

        void Step()
        {
            if (paused || ended) return;
            direction = nextDirection;
            turned = false;
            Point head = new Point(snake[0].X + direction.X, snake[0].Y + direction.Y);
            bool eating = head == food;
            int occupied = snake.Count - (eating ? 0 : 1);
            bool collision = head.X < 0 || head.X >= Columns || head.Y < 0 || head.Y >= Rows;
            for (int i = 0; i < occupied; i++) if (snake[i] == head) collision = true;
            if (collision) { ended = true; timer.Stop(); Invalidate(); return; }
            snake.Insert(0, head);
            if (eating)
            {
                score += 10;
                best = Math.Max(best, score);
                timer.Interval = Math.Max(65, 135 - score / 5);
                PlaceFood();
            }
            else snake.RemoveAt(snake.Count - 1);
            Invalidate();
        }

        void HandleKey(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Escape) { Close(); return; }
            if (e.KeyCode == Keys.R || (ended && e.KeyCode == Keys.Enter)) { Reset(); return; }
            if ((e.KeyCode == Keys.Space || e.KeyCode == Keys.P) && !ended)
            { paused = !paused; Invalidate(); return; }
            if (ended || paused || turned) return;
            Point wanted;
            switch (e.KeyCode)
            {
                case Keys.Up: case Keys.W: wanted = new Point(0, -1); break;
                case Keys.Down: case Keys.S: wanted = new Point(0, 1); break;
                case Keys.Left: case Keys.A: wanted = new Point(-1, 0); break;
                case Keys.Right: case Keys.D: wanted = new Point(1, 0); break;
                default: return;
            }
            if (wanted.X == -direction.X && wanted.Y == -direction.Y) return;
            if (wanted == direction) return;
            nextDirection = wanted;
            turned = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            using (Font title = new Font("Segoe UI", 25, FontStyle.Bold))
                g.DrawString("SNAKE", title, Brushes.White, 20, 14);
            using (Brush muted = new SolidBrush(Color.FromArgb(156, 172, 194)))
            {
                g.DrawString("Eat. Grow. Keep moving.", Font, muted, 24, 63);
                g.DrawString("SCORE  " + score + "     BEST  " + best, Font, muted, 360, 37);
                g.DrawString("Arrows / WASD   Move     Space   Pause     R   Restart     Esc   Quit", Font, muted, 24, TopEdge + Rows * Cell + 17);
            }
            using (Brush tile = new SolidBrush(Color.FromArgb(23, 33, 46)))
            using (Brush alternate = new SolidBrush(Color.FromArgb(25, 36, 49)))
                for (int y = 0; y < Rows; y++)
                    for (int x = 0; x < Columns; x++)
                        g.FillRectangle((x + y) % 2 == 0 ? tile : alternate, LeftEdge + x * Cell, TopEdge + y * Cell, Cell, Cell);
            using (Brush apple = new SolidBrush(Color.FromArgb(255, 112, 119)))
                if (!won) g.FillEllipse(apple, LeftEdge + food.X * Cell + 3, TopEdge + food.Y * Cell + 3, Cell - 6, Cell - 6);
            using (Brush head = new SolidBrush(Color.FromArgb(174, 255, 158)))
            using (Brush body = new SolidBrush(Color.FromArgb(80, 202, 139)))
                for (int i = snake.Count - 1; i >= 0; i--)
                    g.FillRectangle(i == 0 ? head : body, LeftEdge + snake[i].X * Cell + 1, TopEdge + snake[i].Y * Cell + 1, Cell - 2, Cell - 2);
            int cx = LeftEdge + snake[0].X * Cell + Cell / 2 + direction.X * 5;
            int cy = TopEdge + snake[0].Y * Cell + Cell / 2 + direction.Y * 5;
            g.FillEllipse(Brushes.Black, cx - direction.Y * 4 - 2, cy + direction.X * 4 - 2, 4, 4);
            g.FillEllipse(Brushes.Black, cx + direction.Y * 4 - 2, cy - direction.X * 4 - 2, 4, 4);
            if (paused || ended)
            {
                using (Brush shade = new SolidBrush(Color.FromArgb(220, 14, 20, 30)))
                    g.FillRectangle(shade, LeftEdge, TopEdge, Columns * Cell, Rows * Cell);
                using (StringFormat centered = new StringFormat { Alignment = StringAlignment.Center })
                using (Font heading = new Font("Segoe UI", 28, FontStyle.Bold))
                {
                    g.DrawString(ended ? (won ? "YOU WIN!" : "GAME OVER") : "PAUSED", heading, Brushes.White,
                        new RectangleF(LeftEdge, 270, Columns * Cell, 60), centered);
                    g.DrawString(ended ? "Score: " + score + "   |   Press R to play again" : "Press Space to continue", Font, Brushes.White,
                        new RectangleF(LeftEdge, 337, Columns * Cell, 40), centered);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) timer.Dispose();
            base.Dispose(disposing);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SnakeWindow());
        }
    }
}
