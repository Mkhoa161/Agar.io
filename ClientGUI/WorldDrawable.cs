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
///     This class provides the visual representation of the game world.
/// </summary>

using AgarioModels;
using Microsoft.Maui.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace ClientGUI
{
    /// <summary>
    /// Provides the visual representation of the game world. It is responsible for drawing players,
    /// food, and other entities onto a graphical canvas, considering zoom and focus based on the player's position.
    /// </summary>
    class WorldDrawable : IDrawable
    {
        private World _world;
        private float portalX1, portalX2;
        private float portalY1, portalY2;
        private float zoomRatio = 1f;
        private float portalW = 500f, portalH = 500f;

        /// <summary>
        /// Initializes a new instance of the WorldDrawable class that uses the provided world model for drawing.
        /// </summary>
        /// <param name="world">The game world model containing the data needed for rendering.</param>
        public WorldDrawable(World world)
        {
            _world = world;
        }

        /// <summary>
        /// Draws the game world onto the provided canvas. This includes players, foods, and other entities,
        /// appropriately scaling and positioning them based on the player's view and zoom level.
        /// </summary>
        /// <param name="canvas">The canvas on which to draw the game elements.</param>
        /// <param name="dirtyRect">The area of the canvas that needs redrawing, not necessarily used in this context.</param>

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Draw myself
            if (_world.Players.ContainsKey(_world.myID)) {
                var playerX = _world.Players[_world.myID].X;
                var playerY = _world.Players[_world.myID].Y;

                portalW = ((int) _world.Players[_world.myID]._Radius) * 20;
                portalH = portalW;
                zoomRatio = 500 / portalW;

                portalX1 = playerX - (portalW / 2);
                portalX2 = playerX + (portalW / 2);

                portalY1 = playerY - (portalH / 2);
                portalY2 = playerY + (portalH / 2);

                // Drawing the circle for myself

                canvas.FillColor = Color.FromRgb(0, 0, 0);
                canvas.FillCircle((portalW / 2) * zoomRatio, (portalH / 2) * zoomRatio, (_world.Players[_world.myID]._Radius + 1) * zoomRatio);

                canvas.FillColor = Color.FromArgb(_world.Players[_world.myID].ARGBColor.ToString("X"));
                canvas.FillCircle((portalW / 2)*zoomRatio, (portalH / 2) * zoomRatio, _world.Players[_world.myID]._Radius * zoomRatio);

                canvas.FillColor = Color.FromRgb(255, 0, 0);
                canvas.DrawString(_world.Players[_world.myID].Name, (portalW / 2) * zoomRatio, (portalH / 2) * zoomRatio, HorizontalAlignment.Center);
            }

            // Draw Foods
            lock (_world.Foods)
            {
                foreach (var food in _world.Foods.Values)
                {
                    float foodOnPortalX = food.X - portalX1; 
                    float foodOnPortalY = food.Y - portalY1;

                    if (foodOnPortalX > 0 && foodOnPortalX < portalW && foodOnPortalY > 0 && foodOnPortalY < portalH) 
                    {
                        float foodOnScreenX = foodOnPortalX * zoomRatio;
                        float foodOnScreenY = foodOnPortalY * zoomRatio;

                        canvas.FillColor = Color.FromArgb(food.ARGBColor.ToString("X"));
                        canvas.FillCircle(foodOnScreenX, foodOnScreenY, food._Radius * zoomRatio);
                    }
                }
            }

            // Draw other players
            lock (_world.Players)
            {
                foreach (var player in _world.Players.Values)
                {
                    float playerOnPortalX = player.X - portalX1;
                    float playerOnPortalY = player.Y - portalY1;

                    if (playerOnPortalX > 0 && playerOnPortalX < portalW && playerOnPortalY > 0 && playerOnPortalY < portalH)
                    {
                        float playerOnScreenX = playerOnPortalX * zoomRatio;
                        float playerOnScreenY = playerOnPortalY * zoomRatio;

                        canvas.FillColor = Color.FromRgb(0,0,0);
                        canvas.FillCircle(playerOnScreenX, playerOnScreenY, (player._Radius + 1) * zoomRatio);

                        canvas.FillColor = Color.FromArgb(player.ARGBColor.ToString("X"));
                        canvas.FillCircle(playerOnScreenX, playerOnScreenY, player._Radius * zoomRatio);


                        canvas.DrawString(player.Name, playerOnScreenX, playerOnScreenY, HorizontalAlignment.Center);
                    }
                }
            }

        }

    }
}
