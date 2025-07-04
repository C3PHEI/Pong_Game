using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PongServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 8080;
            // Server auf Port 8080 starten
            TcpListener server = new TcpListener(IPAddress.Parse(ip), port);
            server.Start();
            
            Console.WriteLine($"Server läuft auf {ip} : {port}");
            
            while (true)
            {
                // Warte auf Client
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Client verbunden!");
                
                // Handle Client in separatem Task
                _ = Task.Run(() => HandleClient(client));
            }
        }
        
        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            
            while (client.Connected)
            {
                try
                {
                    // Lese Nachricht
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    if (bytes == 0) break;
                    
                    // Konvertiere zu Text
                    string message = Encoding.UTF8.GetString(buffer, 0, bytes).Trim();
                    Console.WriteLine($"Empfangen: {message}");
                    
                    // Echo zurücksenden
                    string echo = $"Echo: {message}\n";
                    byte[] response = Encoding.UTF8.GetBytes(echo);
                    stream.Write(response, 0, response.Length);
                }
                catch
                {
                    break; // Client disconnected
                }
            }
            
            client.Close();
            Console.WriteLine("Client getrennt");
        }
    }
}