﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;

namespace TestServiceB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
