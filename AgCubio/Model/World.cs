using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    /// <summary>
    /// The state of the simulation. Tracks the world width, height, all cubes, etc.
    /// </summary>
    class World
    {
        /// <summary>
        /// Width of the world, in pixels.
        /// </summary>
        public readonly int Width = 1000;
        /// <summary>
        /// Height of the world, in pixels.
        /// </summary>
        public readonly int Height = 1000;

        /// <summary>
        /// All of the cubes in the world.
        /// </summary>
        private Dictionary<long, Cube> cubes;

        public World()
        {

        }

    }
}
