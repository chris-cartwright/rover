using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Newtonsoft.Json;



namespace VehicleLib
{
	/// <summary>
	/// class VehiclePipe
	/// Library to connect, send commands to and recieve signals from a vehicle.
	/// </summary>
	public class VehiclePipe
	{
		// private members
		public delegate void ExceptionHandler(Exception ex);
		private Socket _socket;
		private Dictionary<uint, Query.CallbackHandler> _callbacks;
		private uint _callbackCounter;
		public event ExceptionHandler OnException;
 		public event System.Action OnDisconnect;

		/// http://msdn.microsoft.com/en-us/library/system.net.sockets.addressfamily.aspx
		// TODO - send down the password
		/// <summary>
		/// void Connect(string ip, ushort port)
		/// Connects to a vehicle on a supplied ip and port
		/// Uses any avaible port on local machine
		/// </summary>
		/// <param name="ip">IPv4 address of the vehicle</param>
		/// <param name="port">Port for connection to the vehicle</param>
		//public void Connect(string ip, ushort port) // , string password
		//{
		//    if (_socket != null)
		//    {
		//        throw new ConnectionException("Device already controlled/connected to a client.");
		//    }
		//    const string server = "localhost";
		//    IPEndPoint hostEndPoint;
		//    IPAddress hostAddress = null;
		//    Encoding ASCII = Encoding.ASCII;
		//    Byte[] RecvBytes = new Byte[256];

		//    // Get DNS host information.
		//    IPHostEntry hostInfo = Dns.GetHostEntry(server);
		//    // Get the DNS IP addresses associated with the host.
		//    IPAddress[] IPaddresses = hostInfo.AddressList;

		//    // Evaluate the socket and receiving host IPAddress and IPEndPoint.  
		//    for (int index = 0; index < IPaddresses.Length; index++)
		//    {
		//        hostAddress = IPaddresses[index];
		//        hostEndPoint = new IPEndPoint(hostAddress, port);

		//        // Creates the Socket to send data over a TCP connection.
		//        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		//        // Connect to the host using its IPEndPoint.
		//        _socket.Connect(hostEndPoint);

		//        if (!_socket.Connected)
		//        {
		//            // Connection failed, try next IPaddress.
		//            _socket = null;
		//            continue;
		//        }
		//        _callbackCounter = 0;
		//        _callbacks.Clear();
		//        return;
		//    } // End of the for loop. 

		//    // ?? failed to connect
		//    throw new ConnectionException("Failed to connect.");

		//} // public void Connect()

		public void Connect(IPEndPoint vehicleIPEndPoint, string password)
		{
			if (_socket != null)
			{
				throw new ConnectionException("Device already controlled/connected to a client.");
			}
			//const string server = "localhost";
			IPEndPoint hostEndPoint;
			IPAddress hostAddress = null;
			Encoding ASCII = Encoding.ASCII;
			Byte[] RecvBytes = new Byte[256];

			// Get DNS host information.
			IPHostEntry hostInfo = Dns.GetHostEntry(vehicleIPEndPoint.Address);
			// Get the DNS IP addresses associated with the host.
			IPAddress[] IPaddresses = hostInfo.AddressList;

			// Evaluate the socket and receiving host IPAddress and IPEndPoint.  
			for (int index = 0; index < IPaddresses.Length; index++)
			{
				hostAddress = IPaddresses[index];
				hostEndPoint = new IPEndPoint(hostAddress, vehicleIPEndPoint.Port);

				// Creates the Socket to send data over a TCP connection.
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				// Connect to the host using its IPEndPoint.
				_socket.Connect(hostEndPoint);				

				if (!_socket.Connected)
				{
					// Connection failed, try next IPaddress.
					_socket = null;
					continue;
				}

				Byte[] ByteGet = ASCII.GetBytes(password);
				_socket.Send(ByteGet, ByteGet.Length, 0); // should be sent as a Query object for password validation response from vehicle
				_callbackCounter = 0;
				_callbacks.Clear();
				return;
			}
		}

		/// <summary>
		/// void Diconnect()
		/// Disconnect from a vehicle.
		/// </summary>
		public void Disconnect()
		{
			try
			{
				_socket.Close();

			}
			catch { }
			_socket = null;
		}

		/// <summary>
		/// void SendRaw(object o, uint? callbackID)
		/// Send any Serializable oject to a vehicle.
		/// Oject is serialized using JSON JsonConvert.SerializeObject.
		/// </summary>
		/// <param name="o">ojbect to send</param>
		/// <param name="callbackID">Optional parameter, used when sending an object that expects an object to be returned from the vehicle.</param>
		private void SendRaw(object o, uint? callbackID)
		{
			try
			{
				dynamic packet = new { cmd = o.GetType().Name, data = o };

				if (callbackID != null)
				{
					packet.id = callbackID;
				}

				string s = JsonConvert.SerializeObject(packet);
				Encoding ASCII = Encoding.ASCII;
				Byte[] ByteGet = ASCII.GetBytes(s);
				_socket.Send(ByteGet, ByteGet.Length, 0);
			}
			catch (Exception ex)
			{
				if (!_socket.Connected)
				{
					_socket = null;
				}
				throw new ConnectionException("Failed to send data.", ex);
			}
		}

		/// <summary>
		/// void Send (Query q)
		/// Sends a query to a vehicle requesting a sensor value.
		/// Adds a callback to the callback Dictionary.
		/// Non-blocking, the order the Queries are sent will not neccessarily be the order the Quiers are returned.
		/// </summary>
		/// <param name="q">Query object to send to Rover</param>
		/// Exceptions are allowed to bubble
		public void Send(Query q)
		{
			if (_callbackCounter == uint.MaxValue)
			{
				_callbackCounter = 0;
			}
			++_callbackCounter;
			_callbacks.Add(_callbackCounter, q.Callback);
			SendRaw(q, _callbackCounter);
		}

		/// <summary>
		/// void Send (Action a)
		/// Different actions have different functionality.
		/// Some Actions may persist a device state until a new state is set, while others may cause a device to 
		/// perform an "instance movement" where the device stops once a state is reaches... think of an arm instructed to extend. 
		/// Exceptions are allowed to bubble
		/// </summary>
		/// <param name="q">Action object to send to Rover</param>	
		public void Send(Action a) // might have to be careful here. There is a System.Action 
		{
			SendRaw(a, null);
		}

		/// <summary>
		/// void Recv(string s)
		/// Recieves a string from a vehicle expecting a JSON serialized oject.
		/// Recieved string must contain a [NameSpace.][packet.cmd]. ex. "VDash." + [packet.cmd] 
		/// </summary>
		/// <param name="s">String received from a vehicle.  Should be proper JSON notation.</param>
		private void Recv(string s)
		{
			try
			{
				// Receive the host home page content and loop until all the data is received.
				Byte[] RecvBytes = new Byte[256];
				string strRetPage = "";
				Encoding ASCII = Encoding.ASCII;
				Int32 bytes = _socket.Receive(RecvBytes, RecvBytes.Length, 0);

				strRetPage = strRetPage + ASCII.GetString(RecvBytes, 0, bytes);

				while (bytes > 0)
				{
					bytes = _socket.Receive(RecvBytes, RecvBytes.Length, 0);
					strRetPage = strRetPage + ASCII.GetString(RecvBytes, 0, bytes);
				}
				dynamic packet = JsonConvert.DeserializeObject(strRetPage);

				object receivedOject = Convert.ChangeType(packet.data, Type.GetType("VDash." + packet.cmd));
				if (packet.id != null)
				{
					_callbacks[Convert.ToUInt32(packet.id)](receivedOject);
					return;
				}

				if (packet.cmd.toString().Contains("Exception"))
				{
					if (OnException == null)
					{
						return;
					}
					OnException((VehicleException)receivedOject);
				}				
			}
			catch (TimeoutException)
			{
				// Log error and rethrow
				_socket = null;
				throw new ConnectionException("Connection timed out.");
			}
			catch (Exception ex)
			{
				// Log error and rethrow
				_socket = null;
				throw new ConnectionException("Something went wrong in VehiclePipe.Recv(string s) | " + ex.Message, ex);
			}
		}

		// Properties - getters only
		public Socket Socket
		{
			get { return _socket; }
		}

		public bool Connected
		{
			get { return _socket.Connected; }
		}
	} // end class VehiclePipe
} // end namespace
