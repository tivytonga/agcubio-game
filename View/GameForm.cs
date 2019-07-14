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
    //TODO: Viewport
    //TODO: Handle players with same names
    //TODO: Restart after death
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
        private Rectangle statsRect;
        private const int STATS_WIDTH = 220;
        private bool PlayerAlive = false;
        private string PlayerName;

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
            PlayerName = playerName;

            Size = new Size(world.Width + STATS_WIDTH, world.Height); // todo only want actual game part of display to be that size
            statsRect = new Rectangle(Size.Width - STATS_WIDTH, 0, STATS_WIDTH, Size.Height);
            cubeNamesFont = new Font("Times New Roman", 14);
            infoContainerFont = new Font("Times New Roman", 11, FontStyle.Bold);

            // Send off playerName, wait for data to come back
            state.callback = () => WantMoreData();
            Network.Send(state, PlayerName + "\n");
            WantMoreData();
        }

        /// <summary>
        /// Called when the AgCubio game is opened. The width and height of the Form window is set according
        /// to the width and height parameters in World. The timer starts and keeps track of when the panels
        /// need to re-paint.
        /// </summary>
        private void GUIForm_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = world.Heartbeats_Per_Second;
            timer.Tick += main_Loop;
            timer.Start();
        }

        /// <summary>
        /// This container displays the field of play. Food and players' cubes are in this container
        /// performing various allowed activities: moving, moving over food, and moving over smaller cubes.
        /// </summary>
        private void Game_Paint(object sender, PaintEventArgs e)
        {
            // Don't read and write at same time!
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
            Info_Paint(sender, e);
        }

        /// <summary>
        /// This container displays the current data of the game:
        /// Frames Per Second, Amount of food on the screen, Cube mass, Width of the screen
        /// </summary>
        private void Info_Paint(object sender, PaintEventArgs e)
        {
            // Statistics from World displaying most current data
            string statistics = //"FPS: " + "?" + "\n\n" // TODO: Calculate FPS
                                "Available Food: " + world.foodCount + "\n\n"
                              + "Number of Player Cubes: " + world.playerCount + "\n\n";
            if (PlayerAlive)
            {
                statistics +=
                  "Mass: " +  (int)playerCube.Mass + "\n\n"
                + "Width: " + playerCube.Width;
            }
            // Draw the text and the surrounding rectangle.
            e.Graphics.FillRectangle(Brushes.FloralWhite, statsRect); // todo
            e.Graphics.DrawString(statistics, infoContainerFont, Brushes.Black, statsRect);
        }

        /// <summary>
        /// Called by the timer when it is time to re-paint the form.
        /// </summary>
        private void main_Loop(object sender, EventArgs e)
        {
            Network.Send(state, "(move, " + MousePosition.X + ", " + MousePosition.Y + ")\n");
            Invalidate();
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
                    if (!PlayerAlive)
                    {
                        if (cube.Name == PlayerName)
                        {
                            playerCube = cube;
                            PlayerAlive = true;
                        }
                    }
                    if (cube.id == playerCube.id)
                    {
                        playerCube = cube;
                        if (playerCube.Mass == 0)
                        {
                            PlayerAlive = false;
                            MessageBox.Show("You have died!");
                        }
                    }
                }
            }
            Network.i_want_more_data(state);
        }

        /// <summary>
        /// Called when a key is pressed. If it is the spacebar, sends a 'split' request to the server.
        /// </summary>
        private void gameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                Network.Send(state, "(split, " + playerCube.xCoord + ", " + playerCube.yCoord + ")\n");
            }
        }
    }
}
