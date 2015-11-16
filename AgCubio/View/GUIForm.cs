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
        /// <summary>
        /// World is used to set all the properties of the Form.
        /// Cube is used to set all the properties of the player's cube.
        /// </summary>
        Model.World world;
        Model.Cube cube;
        Point local;

        public GUIForm()
        {
            InitializeComponent();
            world = new Model.World();
            cube = new Model.Cube();
            this.DoubleBuffered = true;
            this.Size = new Size(world.Width, world.Height);
        }

        /// <summary>
        /// This is the form that holds the AgCubio game. The width and height of the Form window is set according
        /// to the width and height parameters in World.
        /// </summary>
        private void GUIForm_Load(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = world.Heartbeats_Per_Second;
            timer.Tick += new EventHandler(main_Loop);
            timer.Start();
        }

        /// <summary>
        /// This container displays the field of play. Food and players' cubes are in this container
        /// performing various allowed activies: moving, moving over food, and moving over smaller cubes.
        /// </summary>
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            // TODO: Code the beginning screen where player enters their name and server
        }

        /// <summary>
        /// Draws a cube that follows the cursor.
        /// </summary>
        private void draw_Player_Cube(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(cube.xCoord, cube.yCoord, cube.Width, cube.Width);
            SolidBrush brush = new SolidBrush(Color.FromName(cube.Color));

            // Draw cube
            e.Graphics.FillRectangle(brush, cube.xCoord, cube.yCoord, cube.Width, cube.Width);
        }

        /// <summary>
        /// This container displays the current data of the game:
        /// Frames Per Second, Amount of food on the screen, Cube mass, Width of the screen
        /// </summary>
        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            // Statistics from World displaying most current data
            string statistics = "FPS: " + world.Heartbeats_Per_Second + "\n\n"
                              + "Food: " + world.Max_Food + "\n\n"
                              + "Mass: " + cube.Mass + "\n\n"
                              + "Width: " + world.Width;

            using (Font font1 = new Font("Times New Roman", 11, FontStyle.Bold, GraphicsUnit.Point))
            {
                Rectangle rect1 = new Rectangle(10, 10, 130, 140);

                // Create a StringFormat object and specify format
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Near;

                // Draw the text and the surrounding rectangle.
                e.Graphics.DrawString(statistics, font1, Brushes.Black, rect1, stringFormat);
            }
        }


        private void main_Loop(object sender, EventArgs e)
        {
            cube.xCoord = local.X;
            cube.yCoord = local.Y;
            this.Invalidate();
        }

        private void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            cube.xCoord = e.X;
            cube.yCoord = e.Y;
            
        }

        private void field_of_play_Paint(object sender, PaintEventArgs e)
        {
            draw_Player_Cube(sender, e);
        }



    }
}
