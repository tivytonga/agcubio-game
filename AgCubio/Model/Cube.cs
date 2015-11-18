using Newtonsoft.Json;
using System;
using System.Drawing;

namespace AgCubio
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
        /// <summary>
        /// The ID of the cube (should be unique).
        /// </summary>
        [JsonProperty("uid")]
        public long id { get; }

        /// <summary>
        /// The team_id will have the same value as the original cube's uid if the cube belongs to that player.
        /// </summary>
        public long team_id { get; }

        /// <summary>
        /// The color of the cube.
        /// </summary>
        [JsonProperty("argb_color")]
        public Color color { get; }

        /// <summary>
        /// The current x position of the top left corner of the cube.
        /// </summary>
        [JsonProperty("loc_x")]
        public int xCoord { get; set; }

        /// <summary>
        /// The current y position of the top left corner of the cube.
        /// </summary>
        [JsonProperty("loc_y")]
        public int yCoord { get; set; }

        /// <summary>
        /// The name of the cube (empty string if not a player i.e. food).
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

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
        public Cube(double loc_x, double loc_y, int argb_color, long uid, long team_id, bool food, string name, double mass)
        {
            id = uid;
            this.team_id = team_id;
            color = Color.FromArgb(argb_color);
            xCoord = (int)loc_x;
            yCoord = (int)loc_y;
            Name = name;
            Mass = mass;
            this.food = food;
        }

        /// <summary>
        /// Creates a new cube (for convenience).
        /// </summary>
        public Cube()
        {
            // TODO: Set defaults of the cube
            Mass = 200;
            xCoord = 0;
            yCoord = 0;
            id = 0;
            Name = "hello";
            color = Color.Black;
            food = true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cube)) return false;
            return (this == (Cube)obj);
        }

        public static bool operator == (Cube first, Cube other)
        {
            if (other.Name == first.Name && other.color == first.color && other.id == first.id)
                return true;
            return false;
        }

        public static bool operator !=(Cube first, Cube other) {
            return !(first == other);
        }


        /// <summary>
        /// Returns the name and unique ID of this cube.
        /// </summary>
        public override string ToString()
        {
            return Name + " " + id;
        }

        /** are the below needed?
       /// <summary>
       /// If this cube is within the field of view, it is visible and
       /// returns true. Otherwise, it disappeared from the field of
       /// view and returns false.
       /// </summary>
       [JsonIgnore]
       public bool Visible { get; set; }

       /// <summary>
       /// If this cube is not visible and its mass is equal to 0, then
       /// it is dead and returns true. Otherwise, this cube is still active and
       /// returns true.
       /// </summary>
       [JsonIgnore]
       public bool Dead;

       /// <summary>
       /// The current status of the cube.
       /// </summary>
       [JsonIgnore]
       public string Status { get; private set; }

       /// <summary>
       /// The time of the last update of this cube's data.
       /// </summary>
       [JsonIgnore]
       public float Last_Update;


      
       /// <summary>
       /// The last known distance from the top of the field of view.
       /// </summary>
       [JsonIgnore]
       public int Top
       {
           get { return 0; } // TODO: Calculate "Top" Property
       }

       /// <summary>
       /// The last known distance from the left of the field of view.
       /// </summary>
       [JsonIgnore]
       public int Left
       {
           get { return 0; } // TODO: Calculate "Left" Property
       }

       /// <summary>
       /// The last known distance from the right of the field of view.
       /// </summary>
       [JsonIgnore]
       public int Right
       {
           get { return 0; } // TODO: Calculate "Right" Property
       }

       /// <summary>
       /// The last known distance from the bottom of the field of view.
       /// </summary>
       [JsonIgnore]
       public int Bottom
       {
           get { return 0; } // TODO: Calculate "Bottom" Property
       }
   **/
    }
}
