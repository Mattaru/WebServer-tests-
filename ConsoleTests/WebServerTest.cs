using Web;

namespace ConsoleTests
{
    internal static class WebServerTest
    {
        private const string URL = $"http://localhost:8080/";

        public static void Run()
        {
            var server = new WebServer(8080);
            server.RequestRecived += Server_RequestRecived;
            server.Start();
            
            Console.WriteLine($"Server started: {URL}");
            Console.ReadLine();
        }

        private static void Server_RequestRecived(object? sender, RequestReciverEventArgs e)
        {
            var context = e.Context;

            Console.WriteLine($"{DateTime.Now} | {context.Request.HttpMethod} | {context.Request.Url}");
            
            using var writer = new StreamWriter(context.Response.OutputStream);
            writer.WriteLine("<div><a href='http://google.com/'>" + "Hellow from the our Web Server." + "<a><div>");
        }
    }
}
