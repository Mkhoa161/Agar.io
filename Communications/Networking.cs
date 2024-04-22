/// <summary>
/// Date: 31-Mar-2024
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
///     This class contains code for networking functionality, including handling connections,
///     sending and receiving messages, and managing client-server interactions.
/// </summary>

using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Communications
{
    /// <summary>
    ///  Handles networking functionality such as connections, sending, and receiving messages.
    /// </summary>
    public class Networking
    {
        // Declear needed variables
        private readonly ILogger _logger;
        private readonly ReportConnectionEstablished _onConnect;
        private readonly ReportDisconnect _onDisconnect;
        private readonly ReportMessageArrived _onMessage;
        private TcpClient _tcpClient;
        private string id = string.Empty;
        private bool waitingForClients;
        private int port;
        private CancellationTokenSource _WaitForCancel = new();
        private char terminationChar = '\n';

        /// <summary>
        /// Construct a Networking object
        /// </summary>
        /// <param name="logger">logger to use</param>
        /// <param name="onConnect">method handle successful connections</param>
        /// <param name="onDisconnect">method handle disconnections</param>
        /// <param name="onMessage">method handle complete messages</param>
        public Networking(ILogger logger,
            ReportConnectionEstablished onConnect,
            ReportDisconnect onDisconnect,
            ReportMessageArrived onMessage)
        {
            _logger = logger;
            _onConnect = onConnect;
            _onMessage = onMessage;
            _onDisconnect = onDisconnect;
            _tcpClient = new TcpClient();
        }

        /// <summary>
        ///   <para>
        ///     A Unique identifier for the entity on the "other end" of the wire.
        ///   </para>
        ///   <para>
        ///     The default ID is the tcp client's remote end point, but you can change it
        ///     if desired, to something like: "Jim"  (for a servers connection to the Jim client)
        ///   </para>
        /// </summary>
        public string ID
        {
            get
            {
                if (id.Length == 0)
                {
                    id = RemoteAddressPort;
                }

                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        ///   True if there is an active connection.
        /// </summary>
        public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

        /// <summary>
        ///   <remark>
        ///     Only useful for server type programs.
        ///   </remark>
        ///   
        ///   <para>
        ///     Used by server type programs which have a port open listening
        ///     for clients to connect.
        ///   </para>
        ///   <para>
        ///     True if the connect loop is active.
        ///   </para>
        /// </summary>
        public bool IsWaitingForClients
        {
            get
            {
                return waitingForClients;
            }
            private set
            {
                waitingForClients = value;
            }
        }

        /// <summary>
        ///   <para>
        ///     When connected, return the address/port of the program we are talking to,
        ///     which is the tcpClient RemoteEndPoint.
        ///   </para>
        ///   <para>
        ///     If not connected then: "Disconnected". Note: if previously was connected, you should
        ///     return "Old Address/Port - Disconnected".
        ///   </para>
        ///   <para>
        ///     If waiting for clients (ISWaitingForClients is true) 
        ///     return "Waiting For Connections on Port: {Port}".  Note: probably shouldn't call this method
        ///     if you are a server waiting on clients.... use the LocalAddressPort method.
        ///   </para>
        /// </summary>
        public string RemoteAddressPort
        {
            get
            {
                if (IsWaitingForClients)
                {
                    return $"Waiting For Connections on Port: {port}";
                }

                else if (_tcpClient.Connected)
                {
                    return $"{_tcpClient.Client.RemoteEndPoint}";
                }

                else if (_tcpClient.Client != null)
                {
                    return $"{_tcpClient.Client.RemoteEndPoint} - Disconnected";
                }

                else
                {
                    return "Disconnected";
                }
            }
        }

        /// <summary>
        ///   <para>
        ///     When connected, return the address/port on this machine that we are talking on.
        ///     which is the tcpClient LocalEndPoint.
        ///   </para>
        ///   <para>
        ///     If not connected then: "Disconnected". Note: if previously was connected, you should
        ///     return "Old Address/Port - Disconnected".
        ///   </para>
        ///   <para>
        ///     If waiting for clients (ISWaitingForClients is true) 
        ///     return "Waiting For Connections on Port: {Port}"
        ///   </para>
        /// </summary>
        public string LocalAddressPort
        {
            get
            {
                if (_tcpClient.Connected) return $"{_tcpClient.Client.LocalEndPoint}";
                else if (IsWaitingForClients) return $"Waiting For Connections on Port: {port}";
                else if (_tcpClient.Client != null) return $"{_tcpClient.Client.LocalEndPoint} - Disconnected";
                else return "Disconnected";
            }
        }

        /// <summary>
        ///   <para>
        ///     Open a connection to the given host/port.  Returns when the connection is established,
        ///     or when an exception is thrown.
        ///   </para>
        ///   <para>
        ///     Note: Servers will not call this method.  It is used by clients connecting to
        ///     a program that is waiting for connections.
        ///   </para>
        ///   <para>
        ///     If the connection happens to already be established, this is a NOP (i.e., nothing happens).
        ///   </para>
        ///   <para>
        ///     For the implementing class, the signature of this method should use async.
        ///   </para>
        ///   <remark>
        ///     This method will have to create and use the low level C# TcpClient class.
        ///   </remark>
        /// </summary>
        /// <param name="host">e.g., 127.0.0.1, or "localhost", or "thebes.cs.utah.edu"</param>
        /// <param name="port">e.g., 11000</param>
        /// <exception cref="Exception"> 
        ///     Any exception caused by the underlying TcpClient object should be handled (logged)
        ///     and then propagated (re-thrown).   For example, failure to connect will result in an exception
        ///     (i.e., when the server is down or unreachable).
        ///     
        ///     See TcpClient documentation for examples of exceptions.
        ///     https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient.-ctor?view=net-7.0#system-net-sockets-tcpclient-ctor
        /// </exception>
        public async Task ConnectAsync(string host, int port)
        {
            if (_tcpClient.Client != null && _tcpClient.Connected) return;

            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(host, port);
                _logger.LogInformation($"Connected to {host}:{port}");

                _onConnect(this); // Inform about successful connection
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to connect to {host}:{port}");
                throw; // Re-throw the exception to propagate it
            }
        }

        /// <summary>
        ///   <para>
        ///     Close the TcpClient connection between us and them.
        ///   </para>
        ///   <para>
        ///     Important: the reportDisconnect handler will _not_ be called (because if your code
        ///     is calling this method, you already know that the disconnect is supposed to happen).
        ///   </para>
        ///   <para>
        ///     Note: on the SERVER, this does not stop "waiting for connects" which should be stopped first with: StopWaitingForClients
        ///   </para>
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    var prevRemoteAddress = RemoteAddressPort;
                    _tcpClient.Close();
                    _logger.LogInformation($"Disconnected from {prevRemoteAddress}");
                }
                else
                {
                    _logger.LogWarning("Disconnect called, but not currently connected");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while disconnecting");
            }
        }

        /// <summary>
        ///   <para>
        ///     Precondition: Networking socket has already been connected.
        ///   </para>
        ///   <para>
        ///     Used when one side of the connection waits for a network messages 
        ///     from a the other (e.g., client -> server, or server -> client).
        ///     Usually repeated (see infinite).
        ///   </para>
        ///   <para>
        ///     Upon a complete message (based on terminating character, '\n') being received, the message
        ///     is "transmitted" to the _handleMessage function.  Upon successfully handling one message,
        ///     if multiple messages are "queued up", continue to send them (one after another)until no 
        ///     messages are left in the stored buffer.
        ///   </para>
        ///   <para>
        ///     Once all data/messages are processed, continue to wait for more data (and repeat).
        ///   </para>
        ///   <para>
        ///     If the TcpClient stream's ReadAsync is "interrupted" (by the connection being closed),
        ///     the stored handle disconnect delegate will be called and this function will end.  
        ///   </para>        
        ///   <para>
        ///     Note: This code will "await" network activity and thus the _handleMessage (and 
        ///     _handleDisconnect) methods are never guaranteed to be run on the same thread, nor are
        ///     they guaranteed to use the same thread for subsequent executions.
        ///   </para>
        /// </summary>
        /// 
        /// <param name="infinite">
        ///    if true, then continually await new messages. If false, stop after first complete message received.
        ///    Thus the "infinite" handling will never return (until the connection is severed).
        /// </param>
        public async Task HandleIncomingDataAsync(bool infinite = true)
        {
            _logger.LogInformation("Enter Handle Incoming Data");
            try
            {
                StringBuilder dataBacklog = new StringBuilder();
                byte[] buffer = new byte[4096];

                NetworkStream stream = _tcpClient.GetStream();

                if (stream == null) return;

                while (infinite)
                {
                    _logger.LogInformation("Enter Handle Incoming Data Loop");

                    int totalNumBytes = await stream.ReadAsync(buffer);

                    if (totalNumBytes == 0) throw new Exception();
                    string data = Encoding.UTF8.GetString(buffer, 0, totalNumBytes);
                    dataBacklog.Append(data);

                    _logger.LogInformation($"Received {totalNumBytes} new bytes for data: {data}");

                    // Handle Message
                    if (MessageHandler(dataBacklog, out string chosenMessage))
                    {
                        try
                        {
                            _logger.LogInformation($"Chosen Message: {chosenMessage}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Logger error: {ex.Message}");
                        }

                        _onMessage(this, chosenMessage);
                    }

                }
            }
            catch (Exception)
            {
                _onDisconnect(this);
            }
        }

        /// <summary>
        /// Given an inputted StringBuilder object, check if this string contains one 
        /// or multiple messages.
        /// </summary>
        /// <param name="data">data to be checked</param>
        /// <param name="message">the chosen message to be read</param>
        /// <returns></returns>
        private bool MessageHandler(StringBuilder data, out string message)
        {
            string Data = data.ToString(); // Convert from StringBuilder to String

            // Find termination index
            int terminator_index = Data.IndexOf(terminationChar);

            _logger.LogInformation($"Termination Character's index was extracted. Index: {terminator_index}");

            if (terminator_index >= 0)
            {
                message = Data.Substring(0, terminator_index);
                data.Remove(0, terminator_index + 1);

                return true;
            }

            else
            {
                message = string.Empty;
                return false;
            }
        }

        /// <summary>
        ///   <para>
        ///     Send a message across the channel (i.e., the TCP Client Stream).  This method
        ///     uses WriteAsync and the await keyword.
        ///   </para>
        ///   <para>
        ///     Important: If the message contains the termination character (TC) (e.g., '\n') it is
        ///     considered part of a **single** message.  All instances of the TC will be replaced with the 
        ///     characters "\\n".
        ///   </para>
        ///   <para>
        ///     If an exception is raised upon writing a message to the client stream (e.g., trying to
        ///     send to a "disconnected" recipient) it must be caught, and then 
        ///     the _reportDisconnect method must be invoked letting the user of this object know 
        ///     that the connection is gone. No exception is thrown by this function.
        ///   </para>
        ///   <para>
        ///     If the connection has been closed already, the send will simply return without
        ///     doing anything.
        ///   </para>
        ///   <para>
        ///     Note: messages are encoded using UTF8 before being sent across the network.
        ///   </para>
        ///   <para>
        ///     For the implementing class, the signature of this method should use "async Task SendAsync(string text)".
        ///   </para>
        ///   <remark>
        ///     Will use the stored tcp object's stream's writeasync method.
        ///   </remark>
        /// </summary>
        /// <param name="text"> 
        ///   The entire message to send. Note: this string may contain the Termination Character '\n', 
        ///   but they will be replaced by "\\n".  Upon receipt, the "\\n" will be replaced with '\n'.
        ///   Regardless, it is a _single_ message from the Networking libraries point of view.
        /// </param>
        public async Task SendAsync(string text)
        {
            try
            {
                text = text.Replace(terminationChar.ToString(), "\\n");
                text += "\n";

                byte[] data = Encoding.UTF8.GetBytes(text);

                NetworkStream stream = _tcpClient.GetStream();

                if (stream == null)
                {
                    _logger.LogError($"Client is not connected.");
                    return;
                }

                await stream.WriteAsync(data, 0, data.Length);

                _logger.LogInformation($"Message was sent: {text}");

            }
            catch (Exception ex)
            {
                // Call the onDisconnect Callback 
                _onDisconnect(this);

                _logger.LogError($"Send Error: {ex.Message}");
            }
        }

        /// <summary>
        ///   <para>
        ///     Stop listening for connections.  This is achieved using the Cancellation Token Source that
        ///     was attached to the tcplistner back in the wait for clients method.
        ///   </para>
        ///   <para>
        ///     This code allows for graceful termination of the program, such as if a disconnect button
        ///     is pressed on a GUI.
        ///   </para>
        ///   <para>
        ///     This code should be a very simple call to the Cancel method of the appropriate cancellation token
        ///   </para>
        /// </summary>
        public void StopWaitingForClients()
        {
            _WaitForCancel.Cancel();
            _logger.LogInformation("Stop waiting for clients.");
        }

        /// <summary>
        ///   Stop listening for messages.  This is achieved using the Cancellation Token Source.
        ///   This allows for graceful termination of the program. This method should also be very simple
        ///   utilizing the cancellation token associated with the ReadAsync method used in the
        ///   HandleIncomingData method
        /// </summary>
        public void StopWaitingForMessages()
        {
            _WaitForCancel.Cancel(); //TODO: not sure
            _logger.LogInformation("Stopped waiting for incoming messages.");
        }

        /// <summary>
        ///   <para>
        ///     This method is only used by Server applications.
        ///   </para>
        ///   <para>
        ///     Handle client connections;  wait for network connections using the low level
        ///     TcpListener object.  When a new connection is found:
        ///   </para>
        ///   <para> 
        ///     IMPORTANT: create a new thread to handle communications from the new client.  
        ///   </para>
        ///   <para>
        ///     This routine runs indefinitely until stopped (could accept many clients).
        ///     Important: The TcpListener should have a cancellationTokenSource attached to it in order
        ///     to allow for it to be shutdown.
        ///   </para>
        ///   <para>
        ///     Important: you will create a new Networking object for each client.  This
        ///     object should use the original call back methods instantiated in the servers Networking object. 
        ///     The new networking object will need to store the new tcp client object returned from the tcp listener.
        ///     Finally, the new networking object (on its new thread) should HandleIncomingDataAsync
        ///   </para>
        ///   <para>
        ///     Again: All connected clients will "share" the same onMessage and 
        ///     onDisconnect delegates, so those methods had better handle this Race Condition.  (IMPORTANT: 
        ///     the locking does _not_ occur in the networking code.)
        ///   </para>
        ///   <para>
        ///     For the implementing class, the signature of this method should use async.
        ///   </para>
        /// </summary>
        /// <param name="port"> Port to listen on </param>
        /// <param name="infinite"> If true, then each client gets a thread that read an infinite number of messages</param>
        public async Task WaitForClientsAsync(int port, bool infinite)
        {
            TcpListener network_listener = new TcpListener(IPAddress.Any, port);

            try
            {
                network_listener.Start();

                _WaitForCancel = new();

                while (infinite)
                {
                    Networking network1 = new Networking(_logger, _onConnect, _onDisconnect, _onMessage);
                    Networking network2 = network1;
                    network2._tcpClient = await network_listener.AcceptTcpClientAsync(_WaitForCancel.Token);
                    network1._onConnect(network1);


                    // Start a new Thread
                    new Thread(() => network1.HandleIncomingDataAsync(infinite)).Start();

                    _logger.LogInformation($"\nConnection: Accepted From {network1._tcpClient.Client.RemoteEndPoint} to {network2._tcpClient.Client.LocalEndPoint}\n");
                }

            }
            catch (Exception ex)
            {
                network_listener.Stop();
                _logger.LogError("Error wait for clients.");
            }
        }


        /// <summary>
        ///   A method that will be called by the networking code when a complete message comes across the channel.
        /// </summary>
        /// <param name="channel">The Networking Object that received the message</param>
        /// <param name="message">The message itself (of course without the terminating protocol character).</param>
        public delegate void ReportMessageArrived(Networking channel, string message);

        /// <summary>
        ///   <para>
        ///     A method that will be called by the networking object when the channel is disconnected.
        ///   </para>
        ///   <para>
        ///     Usage: an outside code base (e.g., a web browser) will be using a Networking object
        ///     to communicate (e.g., with a web server).  If the web server "goes down" (or isn't up
        ///     in the first place) the networking object will call this function so the outside program can take
        ///     the appropriate action (e.g., put up a "504" web page).
        ///   </para>
        /// </summary>
        /// <param name="channel"> The networking object. </param>
        public delegate void ReportDisconnect(Networking channel);

        /// <summary>
        ///  <para>
        ///    The Networking object will call this method when the program is connected to.
        ///  </para>
        ///  <para>
        ///    If the Networking object represents a client asking for a connection to a server
        ///    this method will be called when the connection is established (after connectasync is successful).
        ///  </para>
        ///  <para>
        ///    If the Networking object is being used by a SERVER waiting for client connects, then this
        ///    method will also be called (once for each client connect).
        ///  </para>
        /// </summary>
        /// <param name="channel">The Networking Object itself</param>
        public delegate void ReportConnectionEstablished(Networking channel);
    }
}
