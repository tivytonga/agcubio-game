using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgCubio;

namespace View
{
    public partial class GUIForm : Form
    {
        /// <summary>
        /// World is used to set all the properties of the Form.
        /// Cube is used to set all the properties of the player's cube.
        /// </summary>
        World world;
        Cube cube;
        Point local; // TODO: (Might not need this) Used to keep track of the mouse's location


        /// <summary>
        /// Constructor for AgCubio's GUI. Initializes World and Cube.
        /// DoubleBuffered is set to true to avoid flickering.
        /// The size of the Window is set to World's default.
        /// </summary>
        public GUIForm()
        {
            InitializeComponent();
            world = new World();
            cube = new Cube();
            DoubleBuffered = true;
            Size = new Size(world.Width, world.Height);
        }

        /// <summary>
        /// Called when the AgCubio game is opened. The width and height of the Form window is set according
        /// to the width and height parameters in World. The timer starts and keeps track of when the panels
        /// need to re-paint.
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
            // Settings for the Rectangle and Brush
            Rectangle rect = new Rectangle(cube.xCoord, cube.yCoord, cube.Width, cube.Width);
            SolidBrush brush = new SolidBrush(cube.color);

            // Draw cube
            e.Graphics.FillRectangle(brush, cube.xCoord, cube.yCoord, cube.Width, cube.Width);
            
        }

        /// <summary>
        /// Draws a cube with the specified size and color.
        /// </summary>
        private void draw_Player_Cube(object sender, PaintEventArgs e)
        {
            // Settings for the Rectangle and Brush
            Rectangle rect = new Rectangle(cube.xCoord, cube.yCoord, cube.Width, cube.Width);
            SolidBrush brush = new SolidBrush(cube.color);

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
            string statistics = "FPS: " + "?" + "\n\n" // TODO: Calculate FPS
                              + "Food: " + "?" + "\n\n" // TODO: Calculate # of food on the screen
                              + "Mass: " + cube.Mass + "\n\n"
                              + "Width: " + world.Width;

            using (Font font1 = new Font("Times New Roman", 11, FontStyle.Bold, GraphicsUnit.Point))
            {
                Rectangle rect1 = new Rectangle(10, 10, 130, 140);

                // Create a StringFormat object and specify format (left-allign)
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Near;

                // Draw the text and the surrounding rectangle.
                e.Graphics.DrawString(statistics, font1, Brushes.Black, rect1, stringFormat);
            }
        }

        /// <summary>
        /// Called when the mouse moves within the game view panel.
        /// </summary>
        private void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            cube.xCoord = e.X;
            cube.yCoord = e.Y;
        }

        /// <summary>
        /// This panel lies on top of the game view panel. This panel displays two textboxes:
        /// Player Name textbox allows player to enter a name for their cube. 
        /// Server allows player to enter the server to use for the gameplay.
        /// After the player enters a Player Name and Server, this panel disappears and reveals
        /// the game view panel.
        /// </summary>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            panel1.Size = new Size(world.Width, world.Height);
        }

        /// <summary>
        /// Called when the 'Enter' key has been pressed after text has been
        /// typed in the Player Name textbox.
        /// </summary>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                /// Player entered data for Player Name and Server
                if (!textBox1.Text.Equals("") && !textBox2.Text.Equals("")) // TODO: Check to make sure Server textbox has valid input
                {
                    cube.Name = textBox1.Text;
                    panel1.Visible = false;
                    Refresh();
                }

                // TODO: Take user's input and do something with it
            }
        }

        /// <summary>
        /// Called by the timer when it is time to re-paint the form.
        /// </summary>
        private void main_Loop(object sender, EventArgs e)
        {
                cube.xCoord = local.X;
                cube.yCoord = local.Y;
                splitContainer1.Refresh();
        }

    }
}
