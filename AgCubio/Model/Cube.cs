using Newtonsoft.Json;
using System;
using System.Drawing;

namespace Model
{
    /// <summary>
    /// Stores all information associated with a given cube.
    /// </summary>
    public class Cube
    {
        /// <summary>
        /// The ID of the cube (should be unique).
        /// </summary>
        [JsonProperty("uid")]
        public long id { get; }

        /// <summary>
        /// The color of the cube.
        /// </summary>
        [JsonProperty("argb_color")]
        public Color color { get; }

        /// <summary>
        /// The current x position of the top left corner of the cube.
        /// </summary>
        [JsonProperty("loc_x")]
        public double xPos { get; set; }

        /// <summary>
        /// The current y position of the top left corner of the cube.
        /// </summary>
        [JsonProperty("loc_y")]
        public double yPos { get; set; }

        /// <summary>
        /// The name of the cube (empty string if not a player i.e. food).
        /// </summary>
        [JsonProperty]
        public string Name { get; }
            
        /// <summary>
        /// The current mass of the cube.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// Whether or not the cube is food (not a player).
        /// </summary>
        public bool food { get; }
        
        /// <summary>
        /// The width of the cube (square root of the mass).
        /// </summary>
        [JsonIgnore]
        public int Width { get { return (int)Math.Sqrt(Mass); } }

        /// <summary>
        /// Creates a cube with all of the given properties.
        /// </summary>
        [JsonConstructor]
        public Cube(long uid, int argb_color, double x, double y, string name, double mass, bool food) 
        {
            id = uid;
            color = Color.FromArgb(argb_color);
            xPos = x;
            yPos = y;
            Name = name;
            Mass = mass;
            this.food = food;
        }
    }
}
