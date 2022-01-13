using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;

        public HttpServer(string _ipAddress, int _port)
        {
            ipAddress = IPAddress.Parse(_ipAddress);
            port = _port;

            serverListener = new TcpListener(ipAddress, port);
        }

        public void Start()
        {
            serverListener.Start();
            Console.WriteLine($"Server started on {port}");
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient();
                var networkStream = connection.GetStream();

                var requestText = ReadRequest(networkStream);
                Console.WriteLine(requestText);

                WriteResponse(networkStream, "Hello from the server!");

                connection.Close();
            }
        }

        private void WriteResponse(NetworkStream stream, string message)
        {
            int contentLength = Encoding.UTF8.GetByteCount(message);

            string response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength}

{message}";

            var responseBytes = Encoding.UTF8.GetBytes(response);

            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        private string ReadRequest(NetworkStream stream)
        {
            int bufferLength = 1024;
            var buffer = new byte[bufferLength];

            int totalBytes = 0;

            StringBuilder requestBuilder = new StringBuilder();

            do
            {
                var bytesRead = stream.Read(buffer, 0, bufferLength);
                totalBytes += bytesRead;

                if (totalBytes > 10 * 1024)
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            } while (stream.DataAvailable);

            return requestBuilder.ToString();
        }
    }
}