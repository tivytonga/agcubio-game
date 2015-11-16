﻿using System;
using System.Collections.Generic;
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
        /// All of the cubes in the world.
        /// </summary>
        private Dictionary<long, Cube> cubes;

        /// <summary>
        /// Creates a World with presets: width of 1000, height of 500, 
        /// and Heartbeats_Per_Second of 100000.
        /// </summary>
        public World()
        {
            // TODO: Set the defaults for World constructor
            Width = 1000;
            Height = 500;
            Heartbeats_Per_Second = 100000;
        }

        /// <summary>
        /// Creates a World with given values for width and height.
        /// </summary>
        public World(int width, int height)
        {
            // TODO: Set the defaults for World constructor
            Width = width;
            Height = height;
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