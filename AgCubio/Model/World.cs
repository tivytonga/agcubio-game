using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// The state of the simulation. Tracks the world width, height, all cubes, etc.
    /// </summary>
    class World
    {
        /// <summary>
        /// Width of the world, in pixels.
        /// </summary>
        public static readonly int Width = 1000;
        /// <summary>
        /// Height of the world, in pixels.
        /// </summary>
        public static readonly int Height = 1000;

        /// <summary>
        /// All of the cubes in the world.
        /// </summary>
        private List<Cube> cubes;

        public World()
        {

        }

    }
}
