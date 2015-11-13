using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    /// <summary>
    /// Represents the "state" of the simulation. World is responsible for tracking
    /// the width and height of the world and all the cubes in the game. 
    /// </summary>
    class World
    {
        /// Variables for the properties of the World
        public double width { get; private set; }
        public double height { get; private set; }

        /// <summary>
        /// Creates a World...
        /// </summary>
        public World()
        {
            // TODO: Set the defaults for World
        }



    }
}
