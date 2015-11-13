using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// A Cube object...
    /// </summary>
    public class Cube
    {
        /// Variables for the properties of this cube.
        private int uniqueID;
        private double xCoord;
        private double yCoord;
        private string color;
        public string name { get; private set; }
        private double mass;
        private bool visible;
        private bool dead;
        private string status;
        private float lastUpdate;

        /// <summary>
        /// Creates a new cube.
        /// </summary>
        Cube()
        { }

        /// <summary>
        /// The name of this cube given by the player.
        /// </summary>
        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        /// <summary>
        /// The unique ID associated with this cube.
        /// </summary>
        public int UniqueID
        {
            get { return uniqueID; }
            private set { }
        }

        /// <summary>
        /// The current mass of this cube.
        /// </summary>
        public double Mass
        {
            get { return mass; }
            set { }
        }

        /// <summary>
        /// The current width of this cube.
        /// </summary>
        public double Width
        {
            get { return Math.Sqrt(mass); }
        }

        /// <summary>
        /// The color of this cube.
        /// </summary>
        public string Color
        {
            get { return color; }
            private set { color = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// The last known x-coordinate of this cube.
        /// </summary>
        public double XCoord
        {
            get { return xCoord; }
            set { xCoord = value; }
        }

        /// <summary>
        /// The last known y-coordinate of this cube.
        /// </summary>
        public double YCoord
        {
            get { return yCoord; }
            set { yCoord = value; }
        }

        /// <summary>
        /// The last known distance from the top of the field of view.
        /// </summary>
        public double Top
        {
            get { return 0; } // TODO: Calculate "Top" Property
        }

        /// <summary>
        /// The last known distance from the left of the field of view.
        /// </summary>
        public double Left
        {
            get { return 0; } // TODO: Calculate "Left" Property
        }

        /// <summary>
        /// The last known distance from the right of the field of view.
        /// </summary>
        public double Right
        {
            get { return 0; } // TODO: Calculate "Right" Property
        }

        /// <summary>
        /// The last known distance from the bottom of the field of view.
        /// </summary>
        public double Bottom
        {
            get { return 0; } // TODO: Calculate "Bottom" Property
        }

        /// <summary>
        /// If this cube is within the field of view, it is visible and
        /// returns true. Otherwise, it disappeared from the field of
        /// view and returns false.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        /// <summary>
        /// The time of the last update of this cube's data.
        /// </summary>
        public float LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }

        /// <summary>
        /// If this cube is not visible and its mass is equal to 0, then
        /// it is dead and returns true. Otherwise, this cube is still active and
        /// returns true.
        /// </summary>
        public bool Dead
        {
            get { return dead; } // TODO: Code --> if (!visible) and something else then return dead, else !dead
            set { dead = value; }
        }

        /// <summary>
        /// Prints out the name and unique ID of this cube.
        /// </summary>
        public override string ToString()
        {
            return name + " " + uniqueID;
        }

    }

}
