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
using Newtonsoft.Json;
using System.Diagnostics;

namespace View
{
    public partial class GameForm : Form
    {
        /// <summary>
        /// World is used to set all the properties of the Form.
        /// Cube is used to set all the properties of the player's cube.
        /// </summary>
        private World world;
        private Cube playerCube;
        private PreservedState state;
        private Timer timer;
        private Font cubeNamesFont;
        private Font infoContainerFont;


        /// <summary>
        /// Constructor for AgCubio's GUI. Initializes World and Cube.
        /// DoubleBuffered is set to true to avoid flickering.
        /// The size of the Window is set to World's default.
        /// </summary>
        public GameForm(PreservedState state, string playerName)
        {
            InitializeComponent();
            world = new World();
            this.state = state;
            playerCube = new Cube();
            playerCube.Name = playerName;

            Size = new Size(world.Width, world.Height); // todo only want actual game part of display to be that size
            cubeNamesFont = new Font("Times New Roman", 14);
            infoContainerFont = new Font("Times New Roman", 11, FontStyle.Bold);

            // Send off playerName, wait for data to come back
            state.callback = () => WantMoreData();
            Network.Send(state, playerName + "\n");
            WantMoreData();
        }

        /// <summary>
        /// Called when the AgCubio game is opened. The width and height of the Form window is set according
        /// to the width and height parameters in World. The timer starts and keeps track of when the panels
        /// need to re-paint.
        /// </summary>
        private void GUIForm_Load(object sender, EventArgs e)
        { //todo may not need this method
            timer = new Timer();
            timer.Interval = world.Heartbeats_Per_Second;
            //timer.Tick += main_Loop;
            timer.Start();
        }

        /// <summary>
        /// This container displays the field of play. Food and players' cubes are in this container
        /// performing various allowed activities: moving, moving over food, and moving over smaller cubes.
        /// </summary>
        private void Game_Paint(object sender, PaintEventArgs e)
        {
            lock (world)
            {
                foreach (Cube cube in world.getCubes())
                {
                    // Settings for the Rectangle and Brush
                    Rectangle rect = new Rectangle(cube.xCoord, cube.yCoord, cube.Width, cube.Width);
                    SolidBrush brush = new SolidBrush(cube.color);

                    // Draw cube
                    e.Graphics.FillRectangle(brush, rect);

                    if (!cube.food)
                    {
                        e.Graphics.DrawString(cube.Name, cubeNamesFont, Brushes.Yellow, cube.xCoord, cube.yCoord);
                    }
                }
            }
            statsBox.Invalidate();
        }

        /// <summary>
        /// This container displays the current data of the game:
        /// Frames Per Second, Amount of food on the screen, Cube mass, Width of the screen
        /// </summary>
        private void Info_Paint(object sender, PaintEventArgs e)
        {
            // Statistics from World displaying most current data
            string statistics = //"FPS: " + "?" + "\n\n" // TODO: Calculate FPS
                              "Food: " + world.foodCount + "\n\n"
                              + "Mass: " + playerCube.Mass + "\n\n"
                              + "Width: " + playerCube.Width;
            Rectangle rect1 = new Rectangle(10, 10, 130, 140);

            // Create a StringFormat object and specify format (left-allign)
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Near;

            // Draw the text and the surrounding rectangle.
            e.Graphics.DrawString(statistics, infoContainerFont, Brushes.Black, rect1, stringFormat);
        }

        /// <summary>
        /// Called when the mouse moves within the game view panel.
        /// </summary>
        private void Game_MouseMove(object sender, MouseEventArgs e)
        {
            Network.Send(state, "(move, " + e.X + ", " + e.Y + ")\n");
        }

        /// <summary>
        /// Called by the timer when it is time to re-paint the form.
        /// </summary>
        private void main_Loop(object sender, EventArgs e)
        {
            //if (INGAME)
                //panel1.Visible = false;
            //splitContainer1.Refresh();
        }

        /// <summary>
        /// Called after receiving any data from the server. Asks for more data.
        /// Adds any cubes known to the world.
        /// </summary>
        private void WantMoreData()
        {
            // Must avoid adding to world while reading through cubes, for example   
            lock (world)
            {
                foreach (string datum in state.getLines())
                {
                    Cube cube = JsonConvert.DeserializeObject<Cube>(datum);
                    world.AddCube(cube);
                    if (cube.Name == playerCube.Name)
                    {
                        playerCube = cube;
                        if (playerCube.Mass == 0)
                        {
                            MessageBox.Show("You have died!");
                        }
                    }
                }
            }
            Network.i_want_more_data(state);
            Invalidate();
        }

        /// <summary>
        /// Called when a key is pressed. If it is the spacebar, sends a 'split' request to the server.
        /// </summary>
        private void gameKeyDown(object sender, KeyEventArgs e)
        {
            //todo method not being called........ fix this
            if (e.KeyData == Keys.Space)
            {
                Network.Send(state, "(split, " + playerCube.xCoord + ", " + playerCube.yCoord + ")\n");
            }
        }
    }
}
