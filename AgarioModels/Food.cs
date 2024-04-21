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
///     This class represents the food in an Agario-like game
/// </summary>

using System.Numerics;
using System.Text.Json.Serialization;

namespace AgarioModels
{
    /// <summary>
    /// Represents a food item in the game. This class extends the GameObject class, inheriting its properties such as location,
    /// ID, color, and mass. Food items are consumable entities that players can eat to increase their mass.
    /// </summary>
    public class Food : GameObject
    {
        /// <summary>
        /// Initializes a new instance of the Food class with specified properties.
        /// </summary>
        /// <param name="X">The X-coordinate of the food item in the game world.</param>
        /// <param name="Y">The Y-coordinate of the food item in the game world.</param>
        /// <param name="ID">The unique identifier of the food item.</param>
        /// <param name="ARGBColor">The ARGB color of the food item, used for rendering.</param>
        /// <param name="Mass">The mass of the food item, influencing how much mass a player gains when it is consumed.</param>
        public Food(float X, float Y, long ID, int ARGBColor, float Mass) : base(X, Y, ID, ARGBColor, Mass)
        {
        
        }
    }
}
