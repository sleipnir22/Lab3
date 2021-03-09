using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab2
{
    public partial class Form1 : Form
    {
        World world;
        FlyingObj flobj;
        public Form1()
        {
            InitializeComponent();
            HeightButton.Value = 0;
            SpeedButton.Value = 10;
            AngleButton.Value = 45;
            SizeButton.Value = 0.1M;
            WeightButton.Value = 1M;
            
            //chart1.Series[0].Points.AddXY(0, 0);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            world.Iterate();
            if (world.Obj.Y < 0)
            {
                timer1.Stop();
                button2.Enabled = false;
            }
            label6.Text = world.T.ToString() + " seconds"; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                button2.Enabled = true;
                chart1.Series[0].Points.Clear();
                timer1.Start();
                var y0 = HeightButton.Value;
                var a = AngleButton.Value;
                var v0 = SpeedButton.Value;
                var s = SizeButton.Value;
                var w = WeightButton.Value;
                flobj = new FlyingObj(y0, a, v0, s, w);
                world = new World(flobj, 0.01M, chart1);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Maximum = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.Maximum = trackBar2.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Unpause")
            {
                timer1.Start();
                button2.Text = "Pause";
            }
            else if (timer1.Enabled)
            {
                timer1.Stop();
                button2.Text = "Unpause";
            }
            else return;
        }
    }

    public class World
    {
        decimal _dt;
        public decimal T { get; set; }
        public FlyingObj Obj { get;  }
        const decimal g = 9.81M;
        Chart _chart;
        public World(FlyingObj obj, decimal dt, Chart chart)
        {
            T = 0;
            Obj = obj;
            _dt = dt;
            _chart = chart;
            _chart.Series[0].Points.Clear();
        }

        public void Iterate()
        {
            Obj.Fly(_dt, g);
            _chart.Series[0].Points.AddXY(Obj.X, Obj.Y);
            T += _dt;
        }
    }

    public class FlyingObj
    {
        public readonly decimal v0;
        private readonly double _angle;
        private decimal cosa;
        private decimal sina;
        private decimal _size;
        private decimal _weight;
        private decimal _k;

        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Vx { get; set; }
        public decimal Vy { get; set; }



        public FlyingObj(decimal height, decimal angle, decimal velocity, decimal size, decimal weight)
        {
            Y = height;
            X = 0;
            _angle = (double)angle * Math.PI / 180;
            v0 = velocity;
            cosa = (decimal)Math.Cos(_angle);
            sina = (decimal)Math.Sin(_angle);
            _size = size;
            _weight = weight;
            Vx = v0 * cosa;
            Vy = v0 * sina;
            _k = 0.5M * 0.15M * 1.29M * _size / _weight;

        }

        public void Fly(decimal dt, decimal g)
        {
            decimal V = (decimal)Math.Sqrt((double)(Vx * Vx + Vy * Vy));
            Vx -= _k * Vx * V * dt;
            Vy -= (g + _k * Vy * V) * dt;
            X += Vx * dt;
            Y += Vy * dt;
        }
    }
}
