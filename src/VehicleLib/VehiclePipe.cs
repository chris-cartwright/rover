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
	public class VehiclePipe
	{
		// private members
		public delegate void ExceptionHandler(Exception ex);
		private Socket _socket;
	//	private string _password;
		private Dictionary<uint, Query.CallbackHandler> _callbacks;
		private uint _callbackCounter;
		public event ExceptionHandler OnException;
 		public event System.Action OnDisconnect;

		// http://msdn.microsoft.com/en-us/library/system.net.sockets.addressfamily.aspx
		// TODO - send down the password
		public void Connect(string ip, ushort port) // , string password
		{
			if (_socket != null)
			{
				throw new ConnectionException("Device already controlled/connected to a client.");
			}
			const string server = "localhost";
			IPEndPoint hostEndPoint;
			IPAddress hostAddress = null;
			Encoding ASCII = Encoding.ASCII;
			Byte[] RecvBytes = new Byte[256];

			// Get DNS host information.
			IPHostEntry hostInfo = Dns.GetHostEntry(server);
			// Get the DNS IP addresses associated with the host.
			IPAddress[] IPaddresses = hostInfo.AddressList;

			// Evaluate the socket and receiving host IPAddress and IPEndPoint.  
			for (int index = 0; index < IPaddresses.Length; index++)
			{
				hostAddress = IPaddresses[index];
				hostEndPoint = new IPEndPoint(hostAddress, port);

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
				_callbackCounter = 0;
				_callbacks.Clear();
				return;
			} // End of the for loop. 

			// ?? failed to connect
			throw new ConnectionException("Failed to connect.");

		} // public void Connect()

		public void Disconnect()
		{
			try
			{
				_socket.Close();

			}
			catch { }
			_socket = null;
		}

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

		public void Send(Action a) // might have to be careful here. There is a System.Action 
		{
			SendRaw(a, null);
		}

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
