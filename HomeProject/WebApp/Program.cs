using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApp
{
    public class Program
    {
        public static string? _basePath;
        public static string? Filename;
        public static string? SaveFileName;
        public static void Main(string[] args)
        {
            Filename = _basePath + Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar + "standard.json";
            SaveFileName = _basePath + Path.DirectorySeparatorChar + "SavedGames" + Path.DirectorySeparatorChar + "game.json";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}