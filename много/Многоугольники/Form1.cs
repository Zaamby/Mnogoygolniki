using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Многоугольники
{
    public partial class Form1 : Form
    {
        List<Shape> tochka = new List<Shape>();
        Graphics g;
        int rx, ry, r;
        int del = -1;
        bool flag = false;
        TrackBar radi;
        Random rnd = new Random();
        Form rad;
        Type[] t = new Type[3];
        XmlSerializer formatter;
        UndoRedo undoredo;
        Color c, cout;

        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            Shape strt = new Circle(Size.Width / 2, Size.Height / 2);
            tochka.Add(strt);
            pictureBox1.Invalidate();
            circleToolStripMenuItem.Checked = true;
            t[0] = typeof(List<Circle>);
            t[1] = typeof(List<Square>);
            t[2] = typeof(List<Triangle>);
            formatter = new XmlSerializer(typeof(List<Shape>), t );
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = ".mng";
            openFileDialog1.DefaultExt = ".mng";
            openFileDialog1.Filter = "Многоугольники(*.mng)|*.mng";
            undoredo = new UndoRedo(5);
            undoredo.AddAction(tochka,Shape.Getc,Shape.Getcout,Shape.Getr);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (flag == false)
            {
                for (int i = 0; i < tochka.Count; i++)
                {
                    if (tochka[i].Popali(e.X, e.Y) == true) { flag = true; tochka[i].Getris = true; rx = e.X; ry = e.Y; }
                }
            }
            if (flag == false && tochka.Count > 3 && Shape.Popal(tochka, e.Location) == true)
            {
                flag = true;
                for (int i = 0; i < tochka.Count; i++)
                {
                    tochka[i].Getris = true; rx = e.X; ry = e.Y;
                }
            }
            if (flag == false)
            {
                if (circleToolStripMenuItem.Checked == true)
                {
                    tochka.Add(new Circle(e.X, e.Y));
                    pictureBox1.Invalidate();
                }
                else
                {
                    if (squareToolStripMenuItem.Checked == true)
                    {
                        tochka.Add(new Square(e.X, e.Y));
                        pictureBox1.Invalidate();
                    }
                    else
                    {
                        if (triangleToolStripMenuItem.Checked == true)
                        {
                            tochka.Add(new Triangle(e.X, e.Y));
                            pictureBox1.Invalidate();
                        }
                    }
                }
                for (int i = 0; i < tochka.Count; i++)
                {
                    if (tochka[i].Popali(e.X, e.Y) == true) { flag = true; tochka[i].Getris = true; rx = e.X; ry = e.Y; }
                }
            }

        }

        private void shapeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tochka.Count > 0)
            {
                for (int i = 0; i < tochka.Count; i++)
                {
                    if (tochka[i].Popali(e.X, e.Y) == true) del = i;
                }
                if (del >= 0) tochka.RemoveAt(del);
                del = -1;
                pictureBox1.Invalidate();
                undoredo.AddAction(tochka,Shape.Getc,Shape.Getcout, Shape.Getr);
            }
           
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            for (int i = 0; i < tochka.Count; i++)
            {
                tochka[i].Getx += rnd.Next(-1, 2);
                tochka[i].Gety += rnd.Next(-1, 2);
            }
            pictureBox1.Invalidate();

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            flag = false;
            foreach (Shape a in tochka)
            {
                a.Getris = false;
            }
            if (tochka.Count > 3)
            {
                tochka = Shape.Obolochka(tochka);
            }
            undoredo.AddAction(tochka, Shape.Getc, Shape.Getcout, Shape.Getr);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            g.Clear(Color.White);
            if (tochka.Count > 3)
            {
                for (int i = 0; i < tochka.Count; i++)
                {
                    if (i < tochka.Count - 1)
                        g.DrawLine(new Pen(Color.Black), tochka[i].Getx, tochka[i].Gety, tochka[i + 1].Getx, tochka[i + 1].Gety);
                    else g.DrawLine(new Pen(Color.Black), tochka[i].Getx, tochka[i].Gety, tochka[0].Getx, tochka[0].Gety);
                }
            }
            foreach (Shape a in tochka)
            {
                a.Draw(g);
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            g = pictureBox1.CreateGraphics();
            pictureBox1.Invalidate();
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            circleToolStripMenuItem.Checked = true;
            squareToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = false;
        }

        private void squareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            circleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = true;
            triangleToolStripMenuItem.Checked = false;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            circleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = true;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            Shape.Getc = colorDialog1.Color;
            undoredo.AddAction(tochka, Shape.Getc, Shape.Getcout, Shape.Getr);
        }

        private void borderColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            Shape.Getcout = colorDialog1.Color;
            undoredo.AddAction(tochka, Shape.Getc, Shape.Getcout, Shape.Getr);
        }

        private void radiusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rad = new Form();
            rad.Owner = this;
            rad.ShowIcon = false;
            rad.ShowInTaskbar = false;
            rad.MinimizeBox = false;
            rad.MaximizeBox = false;
            rad.SizeGripStyle = SizeGripStyle.Hide;
            Size l = new Size(200, 100);
            rad.MaximumSize = l;
            rad.MinimumSize = l;
            rad.Size = l;
            radi = new TrackBar();
            radi.Minimum = 5;
            radi.Maximum = 25;
            radi.Value = 10;
            l = new Size(100, 25);
            radi.Size = l;
            radi.Parent = rad;
            radi.Minimum = 5;
            radi.Maximum = 25;
            radi.Value = 10;
            radi.Location = new Point(40, 8);
            rad.Show();
            radi.Scroll += new EventHandler(this.rad_Scroll);
            rad.Disposed += new EventHandler(this.radiusToolStripMenuItem_Disposed);
        }
        private void radiusToolStripMenuItem_Disposed(object sender, EventArgs e)
        {
            undoredo.AddAction(tochka, Shape.Getc, Shape.Getcout, Shape.Getr);
        }
        private void rad_Scroll(object sender, EventArgs e)
        {
            Shape.Getr = radi.Value;
            pictureBox1.Invalidate();

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (tochka.Count > 3)
            {
                tochka = Shape.Obolochka(tochka);
            }
            pictureBox1.Invalidate();
            undoredo.AddAction(tochka, Shape.Getc, Shape.Getcout, Shape.Getr);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            using (TextReader textReader = new StreamReader(openFileDialog1.FileName))
            {
                tochka = (List<Shape>)formatter.Deserialize(textReader);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            using (TextWriter textWriter = new StreamWriter(saveFileDialog1.FileName))
            {
                formatter.Serialize(textWriter, tochka);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            tochka = undoredo.Undo(out c, out cout, out r);
            List<Shape> toc = new List<Shape>();

            Shape l = new Circle();
            foreach (Shape a in tochka)
            {
                if (a is Circle)
                { l = new Circle(a.Getx, a.Gety); }
                if (a is Square) l = new Square(a.Getx, a.Gety);
                if (a is Triangle) l = new Triangle(a.Getx, a.Gety);
                toc.Add(l);
            }
            tochka = toc;
            Shape.Getc = c;
            Shape.Getcout = cout;
            Shape.Getr = r;
            pictureBox1.Invalidate();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            tochka = undoredo.Redo(out c, out cout, out r);
            List<Shape> toc = new List<Shape>();
            Shape l = new Circle();
            foreach (Shape a in tochka)
            {
                if (a is Circle)
                { l = new Circle(a.Getx, a.Gety); }
                if (a is Square) l = new Square(a.Getx, a.Gety);
                if (a is Triangle) l = new Triangle(a.Getx, a.Gety);
                toc.Add(l);
            }
            tochka = toc;
            Shape.Getc = c;
            Shape.Getcout = cout;
            Shape.Getr = r;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {

            if (flag == true)
            {

                foreach (Shape a in tochka)
                {
                    if (a.Getris == true)
                    {
                        a.Getx += e.X - rx;
                        a.Gety += e.Y - ry;
                    }
                }
                pictureBox1.Invalidate();
                rx = e.X; ry = e.Y;
            }
            pictureBox1.Invalidate();
        }
    }
}
