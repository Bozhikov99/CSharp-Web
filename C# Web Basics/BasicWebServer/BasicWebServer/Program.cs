using System.Net;
using System.Net.Sockets;
using System.Text;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 8080;

TcpListener listener = new TcpListener(ipAddress, port);
listener.Start();

while (true)
{
    var connection = listener.AcceptTcpClient();

    Console.WriteLine($"Server started on port {port}");
    Console.WriteLine("Listening for requests...");

    var networkStream = connection.GetStream();

    string content = "Hello from the server!";
    var contentLength = Encoding.UTF8.GetByteCount(content);

    string response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength}

{content}";

    var responseBytes = Encoding.UTF8.GetBytes(response);
    networkStream.Write(responseBytes);
    connection.Close();
}