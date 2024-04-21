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
///     This class represents the game world in an Agario-like game, encapsulating all game logic.
/// </summary>

using Logger;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace AgarioModels
{
    /// <summary>
    /// Represents the game world in an Agario-like game, encapsulating all game logic,
    /// including players, food, and game dimensions. It provides methods to update the game state.
    /// </summary>
    public class World 
    {
        public Dictionary<long, Player> Players { get; set; }
        public Dictionary<long, Food> Foods { get; set; }
        public ILogger _logger;
        public long myID { get; set; }
        public Vector2 myDirection;
  

        public const int Width = 5000;
        public const int Height = 5000;

        /// <summary>
        /// Initializes a new instance of the World class, setting up an empty game state with players and foods,
        /// and initializing logging and default player direction.
        /// </summary>
        /// <param name="logger">The logger to be used for logging game-related information.</param>
        public World(ILogger logger)
        {
            Players = [];
            Foods = [];

            _logger = logger;
            myDirection = new Vector2(50, 25);
        }

        /// <summary>
        /// Advances the game by one step. Updates the position of the player's circle in the game world
        /// by applying the current direction of movement to the player's coordinates.
        /// </summary>
        public void AdvanceGameOneStep()
        {
            if (Players.ContainsKey(myID)) 
            {
                // Update the circle position by adding the direction vector
                Players[myID].X += myDirection.X;
                Players[myID].Y += myDirection.Y;
            }
        }
    }
}
