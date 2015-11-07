using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class GUIForm : Form
    {

        private SolidBrush myBrush;

        public GUIForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
        int count = 0;

        /// <summary>
        /// 
        /// </summary>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Color color = Color.FromArgb(count, count, count);
            myBrush = new SolidBrush(color);

            count++;

            if (count > 255) count = 0;

            e.Graphics.FillRectangle(myBrush, new Rectangle(count, count, 10 + count, 10 + 2 * count));
            Console.WriteLine("repainting " + count);

            Invalidate();
        }
    }
}
