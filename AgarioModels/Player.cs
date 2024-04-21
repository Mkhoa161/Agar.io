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
///     This class represents the player in an Agario-like game
/// </summary>

using System.Numerics;

namespace AgarioModels
{
    /// <summary>
    /// Represents a player in the game. This class extends GameObject to include player-specific properties
    /// such as name. It provides all necessary data for managing a player entity within the game world.
    /// </summary>
    public class Player : GameObject
    {
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the Player class with specified initial settings.
        /// </summary>
        /// <param name="X">The initial X-coordinate of the player in the game world.</param>
        /// <param name="Y">The initial Y-coordinate of the player in the game world.</param>
        /// <param name="ID">The unique identifier for the player.</param>
        /// <param name="ARGBColor">The color of the player used for rendering in ARGB format.</param>
        /// <param name="Mass">The initial mass of the player, affecting game dynamics.</param>
        public Player(float X, float Y, long ID, int ARGBColor, float Mass) : base(X, Y, ID, ARGBColor, Mass)
        {
        }
    }
}
