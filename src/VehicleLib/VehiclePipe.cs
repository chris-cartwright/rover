using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
//using System.Runtime.Serialization.Json;
using Newtonsoft.Json;



namespace VehicleLib
{
	public class VehiclePipe
	{
		// private members
		private Socket _socket;
		private string _password;

		// http://msdn.microsoft.com/en-us/library/system.net.sockets.addressfamily.aspx
		public void Connect(string ip, ushort port)
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
				return;
			} // End of the for loop. 

			// ?? failed to connect
			throw new ConnectionException("Failed to connect.");

		} // public void Connect()

		public void Disconnect()
		{
			_socket.Close();
			_socket = null;
		}

		private void Send(Object o)
		{
			try
			{
				JsonSerializer js = new JsonSerializer();
				JsonWriter jw = null;
				js.Serialize(jw,o);
				string s = jw.ToString();
				Encoding ASCII = Encoding.ASCII;
				Byte[] ByteGet = ASCII.GetBytes(s);
				_socket.Send(ByteGet, ByteGet.Length, 0);
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
				throw new ConnectionException("Something went wrong in VehiclePipe.Send(Object o) | " + ex.Message, ex);
			}
		}

		public void Send (Query q)
		{
			Send((Object)q);
		}

		public void Send(Action a) // might have to be careful here. There is a System.Action 
		{
			Send((Object)a);
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

		public void SetPassword(string password)
		{
			_password = password;
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


	// Extension methods for JSON serialization/deserialization - should move to a generic include?
	// http://www.jarloo.com/serialize-to-json/
	//public static class Extensions
	//{
	//    public static string ToJson<T>(this T obj)
	//    {
	//        MemoryStream stream = new MemoryStream();
	//        try
	//        {
	//            DataContractJsonSerializer jsSerializer = new DataContractJsonSerializer(typeof(T));
	//            jsSerializer.WriteObject(stream, obj);
	//            return Encoding.UTF8.GetString(stream.ToArray());
	//        }
	//        finally
	//        {
	//            stream.Close(); stream.Dispose();
	//        }
	//    }

	//    public static T FromJson<T>(this string input)
	//    {
	//        MemoryStream stream = new MemoryStream();
	//        try
	//        {
	//            DataContractJsonSerializer jsSerializer = new DataContractJsonSerializer(typeof(T));
	//            stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
	//            T obj = (T)jsSerializer.ReadObject(stream);
	//            return obj;
	//        }
	//        finally
	//        {
	//            stream.Close(); stream.Dispose();
	//        }
	//    }
	//}
} // end namespace
