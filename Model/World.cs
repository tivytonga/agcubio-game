using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    /// <summary>
    /// The state of the simulation. Tracks the world width and height of the world and all cubes in the game.
    /// </summary>
    public class World
    {
        /// <summary>
        /// Maps from id to Cube. Every Cube in the World.
        /// </summary>
        private Dictionary<long, Cube> cubes = new Dictionary<long, Cube>();

        public long foodCount { get; private set; }
        public long playerCount { get; private set; }

        /// <summary>
        /// Creates a World with presets: width of 1000, height of 500, 
        /// and Heartbeats_Per_Second of 25.
        /// </summary>
        public World()
        {
            Width = 1000;
            Height = 500;
            Heartbeats_Per_Second = 25;
            foodCount = 0;
            playerCount = 0;
        }

        /// <summary>
        /// Returns all the cubes in the world, mapping id to Cube object.
        /// </summary>
        public IEnumerable<Cube> getCubes()
        {
            return cubes.Values;
        }

        /// <summary>
        /// Adds the given cube to World if not already existing. If existing, then
        /// updates its info. Also updates foodCount and playerCount if applicable.
        /// If mass is 0, deletes the cube.
        /// </summary>
        public void AddCube(Cube cube)
        {
            // Check if removal needed
            if (cube.Mass == 0)
            {
                if (cubes.Remove(cube.id))
                {
                    if (cube.food) foodCount--;
                    else playerCount--;
                }
                return;
            }

            // Add or update Cube
            if (cubes.ContainsKey(cube.id))
                cubes[cube.id] = cube;
            else
            {
                cubes.Add(cube.id, cube);
                if (cube.food) foodCount++;
                else playerCount++;
            }
        }

        /// Variables for the properties of the World
        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        /// The maximum distance a cube can split.
        /// </summary>
        public int Max_Split_Distance { get; set; }

        /// <summary>
        /// The maximum speed a cube can move.
        /// </summary>
        public int Max_Speed { get; set; }

        /// <summary>
        /// The minimum speed a cube can move.
        /// </summary>
        public int Min_Speed { get; set; }

        // TODO: Write summary for AttritionRate
        /// <summary>
        /// 
        /// </summary>
        public int Attrition_Rate { get; set; }

        /// <summary>
        /// The maximum number of food allowed to show up on the screen.
        /// </summary>
        public int Max_Food { get; set; }

        /// <summary>
        /// The value that food can add to a cube's mass.
        /// </summary>
        public int Food_Value { get; set; }

        /// <summary>
        /// The mass a player's cube starts with.
        /// </summary>
        public int Player_Start_Mass { get; set; }

        /// <summary>
        /// The smallest mass a cube can split down to.
        /// </summary>
        public int Min_Split_Mass { get; set; }

        // TODO: Write summary for AbsorbConstant
        /// <summary>
        /// 
        /// </summary>
        public int Absorb_Constant { get; set; }

        // TODO: Write summary for MaxViewRange
        /// <summary>
        /// 
        /// </summary>
        public int Max_View_Range { get; set; }

        // TODO: Write a summary for HeartbeatsPerSecond
        /// <summary>
        /// 
        /// </summary>
        public int Heartbeats_Per_Second { get; set; }

    }
}
