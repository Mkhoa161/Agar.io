/// <summary>
/// Date: 14-Apr-2024
/// Course: CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Khoa Minh Ngo and Duke Nguyen - This work may not
///            be copied for use in Academic Coursework.
/// 
/// We, Khoa Minh Ngo and Duke Nguyen, certify that we wrote this code from scratch and
/// did not copy it in part or whole from another source. All references used in the completion 
/// of the assignments are cited in my README file
/// 
/// File Contents
/// 
///     This class represents the Game Object in an Agario-like game
/// </summary>


using System.Numerics;

namespace AgarioModels
{
    /// <summary>
    /// Represents a base game object within the game world. This class provides common properties
    /// such as location, color, and mass that are shared across different types of game entities like players or food.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the game object.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Private variable to hold the location of the game object in a 2D space.
        /// </summary>
        private Vector2 _Location { get; set; }

        /// <summary>
        /// Public property to get the X-coordinate of the game object. Set operation is not implemented.
        /// </summary>
        public float X { get { return _Location.X; } set { } }

        /// <summary>
        /// Public property to get the Y-coordinate of the game object. Set operation is not implemented.
        /// </summary>
        public float Y { get { return _Location.Y; } set { } }

        /// <summary>
        /// Gets or sets the ARGB color of the game object, used for rendering.
        /// </summary>
        public int ARGBColor { get; set; }

        /// <summary>
        /// Gets or sets the mass of the game object, which can influence its physical behavior and interactions.
        /// </summary>
        public float Mass { get; set; }

        // Radius property calculated from Mass
        public float _Radius => (float) Math.Sqrt(this.Mass / Math.PI);

        /// <summary>
        /// Initializes a new instance of the GameObject class with specified location, ID, color, and mass.
        /// </summary>
        /// <param name="X">The initial X-coordinate of the game object.</param>
        /// <param name="Y">The initial Y-coordinate of the game object.</param>
        /// <param name="ID">The unique identifier of the game object.</param>
        /// <param name="ARGBColor">The ARGB color of the game object.</param>
        /// <param name="Mass">The mass of the game object, affecting its radius.</param>
        public GameObject(float X, float Y, long ID, int ARGBColor, float Mass)
        {
            this.ID = ID;
            _Location = new Vector2(X, Y);
            this.ARGBColor = ARGBColor;
            this.Mass = Mass;
        }
    }
}
