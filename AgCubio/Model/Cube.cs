using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Represents a cube in the game logic. The cube's name is set to the player's input during
    /// the creation of the cube. Also, at the creation of the cube the unique ID, and color are set 
    /// to random values that cannot be changed by the player. The mass of the cube increases as the
    /// cube consumes food or other cubes. The cube's current location, visibility, and status are
    /// tracked in the game. If the cube is no longer active, then it is considered dead.
    /// </summary>
    public class Cube
    {
        /// Variables for the properties of this cube.
   
        /// <summary>
        /// The unique ID associated with this cube.
        /// </summary>
        public int Unique_ID { get; set; }

        /// <summary>
        /// The last known x-coordinate of this cube.
        /// </summary>
        public int xCoord;

        /// <summary>
        /// The last known y-coordinate of this cube.
        /// </summary>
        public int yCoord;

        /// <summary>
        /// The color of this cube.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The name of this cube given by the player.
        /// </summary>
        public string Name;

        /// <summary>
        /// The current mass of this cube.
        /// </summary>
        public int Mass { get; set;}

        /// <summary>
        /// If this cube is within the field of view, it is visible and
        /// returns true. Otherwise, it disappeared from the field of
        /// view and returns false.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// If this cube is not visible and its mass is equal to 0, then
        /// it is dead and returns true. Otherwise, this cube is still active and
        /// returns true.
        /// </summary>
        public bool Dead;

        /// <summary>
        /// The current status of the cube.
        /// </summary>
        public string Status;

        /// <summary>
        /// The time of the last update of this cube's data.
        /// </summary>
        public float Last_Update;

        /// <summary>
        /// Creates a new cube.
        /// </summary>
        public Cube()
        {
            // TODO: Set defaults of the cube
            Mass = 50;
        }

        /// <summary>
        /// The current width of this cube.
        /// </summary>
        public int Width
        {
            get { return (int) Math.Sqrt(Mass); }
            private set { Mass = value * 2; }
        }

        /// <summary>
        /// The last known distance from the top of the field of view.
        /// </summary>
        public int Top
        {
            get { return 0; } // TODO: Calculate "Top" Property
        }

        /// <summary>
        /// The last known distance from the left of the field of view.
        /// </summary>
        public int Left
        {
            get { return 0; } // TODO: Calculate "Left" Property
        }

        /// <summary>
        /// The last known distance from the right of the field of view.
        /// </summary>
        public int Right
        {
            get { return 0; } // TODO: Calculate "Right" Property
        }

        /// <summary>
        /// The last known distance from the bottom of the field of view.
        /// </summary>
        public int Bottom
        {
            get { return 0; } // TODO: Calculate "Bottom" Property
        }

        /// <summary>
        /// Prints out the name and unique ID of this cube.
        /// </summary>
        public override string ToString()
        {
            return Name + " " + Unique_ID;
        }

    }

}
