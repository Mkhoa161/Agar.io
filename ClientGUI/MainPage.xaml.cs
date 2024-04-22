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
///     This class contains code for the Client (Player) GUI for an Agar.io-like game.
/// </summary>

using AgarioModels;
using Communications;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System.Net;
using System.Text.Json;
using System.Numerics;
using System.Diagnostics;
using ABI.System.Numerics;
using Microsoft.Maui.Layouts;


namespace ClientGUI
{
    /// <summary>
    /// Represents the main page of the client GUI for an Agar.io-like game,
    /// handling user interaction, game rendering, and network communication.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private World world;
        private WorldDrawable drawWorld;
        private Networking network;

        private bool alive = false;

        private readonly ILogger _logger;
        private IDispatcherTimer gameTimer;
        private IDispatcherTimer fpsTimer;

        private long myID = 0;
        private int xPos, yPos;
        private int FrameCount = 0;

        private float CurrScore = 0;
        private float ScoreRecord = 0;

        private int TimeSpanStart, TimeSpanEnd;
        private float LifeTimeRecord = 0;

        /// <summary>
        /// Constructor for MainPage initializing components and setting up networking callbacks.
        /// </summary>
        /// <param name="logger">Logger for logging messages.</param>
        public MainPage(ILogger<MainPage> logger)
        {
            InitializeComponent();
            _logger = logger;

            // Initialize the Networking class
            network = new Networking(
                logger,
                OnConnectionEstablished,
                OnDisconnect,
                OnMessageArrived);

            network.ID = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        }

        /// <summary>
        /// Handles incoming network messages and processes them based on their protocol command.
        /// </summary>
        /// <param name="channel">Networking channel through which the message arrived.</param>
        /// <param name="message">The received message.</param>
        /// <exception cref="InvalidOperationException">Thrown when message deserialization fails.</exception>
        private void OnMessageArrived(Networking channel, string message)
        {
            // Receive my ID
            if (message.StartsWith(Protocols.CMD_Player_Object))
            {
                var myIdInfo = message[Protocols.CMD_Player_Object.Length..];
                myID = JsonSerializer.Deserialize<long>(myIdInfo);
            }

            // Receive list of current food
            else if (message.StartsWith(Protocols.CMD_Food))
            {
                var foodInfo = message[Protocols.CMD_Food.Length..];
                var foods = JsonSerializer.Deserialize<List<Food>>(foodInfo) ?? throw new InvalidOperationException("Deserialization failed");

                lock (world.Foods)
                {
                    foreach (var food in foods)
                    {
                        var currID = food.ID;
                        if (!world.Foods.ContainsKey(currID))
                            world.Foods.Add(currID, food);
                        else
                            world.Foods[currID] = food;

                    }
                }
            }

            // Receive list of current players
            else if (message.StartsWith(Protocols.CMD_Update_Players))
            {
                var playerInfo = message[Protocols.CMD_Update_Players.Length..];
                var players = JsonSerializer.Deserialize<List<Player>>(playerInfo) ?? throw new InvalidOperationException("Deserialization failed");

                lock (world.Players)
                {
                    foreach (var player in players)
                    {
                        var currID = player.ID;
                        if (!world.Players.ContainsKey(currID))
                            world.Players.Add(currID, player);
                        else
                            world.Players[currID] = player;

                    }
                }

                if (world.Players.ContainsKey(myID))
                {
                    world.myID = myID;
                }
            }

            // receive list of eaten food
            else if (message.StartsWith(Protocols.CMD_Eaten_Food))
            {
                var eatenFoodInfo = message[Protocols.CMD_Eaten_Food.Length..];
                var eatenFoodList = JsonSerializer.Deserialize<List<long>>(eatenFoodInfo) ?? throw new InvalidOperationException("Deserialization failed");

                lock (world.Foods)
                {
                    foreach (var eatenFoodID in eatenFoodList)
                    {
                        if (world.Foods.ContainsKey(eatenFoodID))
                            world.Foods.Remove(eatenFoodID);
                    }
                }
            }

            // receive list of dead players
            else if (message.StartsWith(Protocols.CMD_Dead_Players))
            {
                var deadPlayersInfo = message[Protocols.CMD_Dead_Players.Length..];
                var deadPlayersList = JsonSerializer.Deserialize<List<long>>(deadPlayersInfo) ?? throw new InvalidOperationException("Deserialization failed");

                lock (world.Players)
                {
                    foreach (var deadPlayerID in deadPlayersList)
                    {
                        if (world.Players.ContainsKey(deadPlayerID))
                            world.Players.Remove(deadPlayerID);

                        // Handle when my player is dead
                        if (deadPlayerID == myID)
                        {
                            Dispatcher.Dispatch(() =>
                            {
                                alive = false;

                                // Display Score (Mass) statistics
                                if (CurrScore > ScoreRecord) { ScoreRecord = CurrScore; }
                                Score.Text = $"Score: {CurrScore}";
                                ScoreRecords.Text = $"(Highest: {ScoreRecord})";

                                // Display Timespan statistics
                                _logger.LogInformation($"TimeEnd: {TimeSpanEnd}");
                                var CurrLifeTime = (float) (TimeSpanEnd - TimeSpanStart) / 10;
                                if (CurrLifeTime > LifeTimeRecord) { LifeTimeRecord = CurrLifeTime; }
                                LifeTime.Text = $"Alive Time: {CurrLifeTime} seconds";
                                LifeTimeRecords.Text = $"(Longest: {LifeTimeRecord} seconds)";

                                Alive.IsVisible = false;
                                GameOver.IsVisible = true;
                            });

                            fpsTimer.Stop();
                        }
                    }
                }

            }

            // receive current heartbeat
            else if (message.StartsWith(Protocols.CMD_HeartBeat))
            {
                Dispatcher.Dispatch(() =>
                {
                    var HBCountString = message[Protocols.CMD_HeartBeat.Length..];
                    var HBCount = JsonSerializer.Deserialize<int>(HBCountString);
                    HB.Text = $"Heartbeat: {HBCount}";

                    if (TimeSpanStart == 0 && alive) { TimeSpanStart = HBCount; _logger.LogInformation($"TimeStart: {TimeSpanStart}"); }
                    
                    if (alive) 
                    { 
                        Split.Focus(); 
                        TimeSpanEnd = HBCount; 
                    }

                });
            }

        }

        /// <summary>
        /// Placeholder for handling network disconnection events.
        /// </summary>
        /// <param name="channel">Networking channel that was disconnected.</param>
        private void OnDisconnect(Networking channel)
        {

        }

        /// <summary>
        /// Handles the restart button click, resetting game state and re-sending start game message to server.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnRestartClicked(object sender, EventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                GameOver.IsVisible = false;
                Alive.IsVisible = true;
            });

            // Send Start_Game message to Server
            var message = String.Format(Protocols.CMD_Start_Game, PlayerNameEntry.Text);
            await network.SendAsync(message);
            fpsTimer.Start();

            // Reset timespan count for stats
            TimeSpanStart = 0;
            TimeSpanEnd = 0;

            alive = true;
            Dispatcher.Dispatch(() => { Split.Focus(); });
            
        }

        /// <summary>
        /// Initializes connection once successfully established, sets up the world, and sends a start game message.
        /// </summary>
        /// <param name="channel">Networking channel that established connection.</param>
        private async void OnConnectionEstablished(Networking channel)
        {
            // Display Game Page
            LoginPage.IsVisible = false;
            GamePage.IsVisible = true;
            Split.Focus();

            world = new World(_logger);
            drawWorld = new WorldDrawable(world);

            InitializeGameLogic();

            // Send Start_Game message to Server
            var message = String.Format(Protocols.CMD_Start_Game, PlayerNameEntry.Text);
            await network.SendAsync(message);

        }

        /// <summary>
        /// Handles the connect button click, setting up connection based on the provided server address and port.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void OnConnectClicked(object sender, EventArgs e)
        {
            string playerName = PlayerNameEntry.Text;
            string serverAddress = ServerAddressEntry.Text;
            string portText = ServerPortEntry.Text;
            int port;

            // Validate the input fields are not empty and the port is a number
            if (string.IsNullOrWhiteSpace(playerName) ||
                string.IsNullOrWhiteSpace(serverAddress) ||
                !int.TryParse(portText, out port))
            {
                StatusMessage.Text = "Please fill in all fields correctly.";
                StatusMessage.TextColor = Colors.Red;
                return;
            }

            // Attempt to connect to the server
            try
            {
                StatusMessage.Text = "Connecting...";
                StatusMessage.TextColor = new Color(255, 255, 255);
                StatusMessage.BackgroundColor = new Color(0, 0, 255);

                await network.ConnectAsync(serverAddress, port);
                new Thread(() => network.HandleIncomingDataAsync(true)).Start();

                Dispatcher.Dispatch(() =>
                {
                    var currPort = network.LocalAddressPort.Remove(0, 19);
                    ServerAddressEntry.Text = $"{Dns.GetHostEntry(Dns.GetHostName()).AddressList[1]}";
                    network.ID = $"127.0.0.1:{currPort}";
                });

                StatusMessage.Text = "Connected";
                StatusMessage.TextColor = new Color(0, 0, 0);
                StatusMessage.BackgroundColor = new Color(0, 255, 0);



            }
            catch (Exception ex)
            {
                StatusMessage.Text = "Error in Networking";
                StatusMessage.TextColor = Colors.Red;
            }
        }

        /// <summary>
        /// Initializes game timers and other necessary game logic components.
        /// </summary>
        private void InitializeGameLogic()
        {
            alive = true;
            PlaySurface.Drawable = drawWorld;

            // Setup and start the game timer
            gameTimer = Dispatcher.CreateTimer(); ;
            gameTimer.Interval = TimeSpan.FromMilliseconds(30); // aim for FPS = 30
            gameTimer.Tick += GameStep;


            fpsTimer = Dispatcher.CreateTimer();
            fpsTimer.Interval = TimeSpan.FromMilliseconds(1000);
            fpsTimer.Tick += UpdateFPS;

            gameTimer.Start();
            fpsTimer.Start();
        }

        /// <summary>
        /// Updates the FPS display based on the number of frames rendered in the last second.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">Event arguments.</param>
        private void UpdateFPS(object sender, EventArgs e)
        {
            int fps = FrameCount;
            Dispatcher.Dispatch(() =>
            {
                FPS.Text = $"FPS: {fps}";
            });

            FrameCount = 0;
        }

        /// <summary>
        /// Game loop step function that advances game logic, renders the frame, and sends position updates.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void GameStep(object? sender, EventArgs e)
        {
            world.AdvanceGameOneStep();
            PlaySurface.Invalidate();
            FrameCount++;

            if (world.Players.ContainsKey(myID))
            {
                //invalidateCount++;
                xPos = (int)(world.Players[myID].X + world.myDirection.X);
                yPos = (int)(world.Players[myID].Y + world.myDirection.Y);

                string message = String.Format(Protocols.CMD_Move, xPos, yPos);
                await network.SendAsync(message);
            }

            UpdateLabels();
        }

        /// <summary>
        /// Updates on-screen labels with current game state information.
        /// </summary>
        private void UpdateLabels()
        {
            if (world.Players.ContainsKey(myID))
            {
                // Assuming you have labels named lblCircleCenter and lblDirection in your XAML
                Dispatcher.Dispatch(() =>
                {
                    CircleCenter.Text = $"Circle Center: {(int)world.Players[myID].X}, {(int)world.Players[myID].Y}";
                    Direction.Text = $"Direction: {(int)world.myDirection.X}, {(int)world.myDirection.Y}";

                    CurrScore = world.Players[myID].Mass;
                    Mass.Text = $"Mass: {world.Players[myID].Mass}";


                });
            }
        }

        /// <summary>
        /// Handles pointer input to change the direction of the player's circle.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">Pointer event arguments containing new pointer location.</param>
        private async void PointerChanged(object? sender, PointerEventArgs e)
        {
            Point? position = e.GetPosition(PlaySurface);
            // Play Surface is the name of the graphics view to get coordinates relative to.
            xPos = (int)position.Value.X;
            yPos = (int)position.Value.Y;


            if (world.Players.ContainsKey(myID))
            {
                world.myDirection = new System.Numerics.Vector2(xPos - 250, yPos - 250);
            }

        }

        /// <summary>
        /// Logs tap events for debugging purposes.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnTap(object? sender, EventArgs e)
        {
            _logger.LogInformation($"OnTapRunning");
        }

        /// <summary>
        /// Split when Spacebar is pressed
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">Event arguments</param>
        private async void OnSpacebarPress(object? sender, EventArgs e)
        {
            var message = String.Format(Protocols.CMD_Split, world.myDirection.X, world.myDirection.Y);
            await network.SendAsync(message);
        }
    }
}
