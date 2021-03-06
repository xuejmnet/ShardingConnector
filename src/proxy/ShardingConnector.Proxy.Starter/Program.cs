using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShardingConnector.Logger;
using ShardingConnector.Merge.Engine;
using ShardingConnector.Proxy.Common;
using ShardingConnector.Proxy.Common.Context;
using ShardingConnector.Proxy.Network;
using ShardingConnector.RewriteEngine.Context;
using ShardingConnector.Route;
using ShardingConnector.ShardingCommon.Core.Rule;

namespace ShardingConnector.Proxy.Starter
{
    class Program
    {
        private static IConfiguration _configuration =
            new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter("Microsoft", LogLevel.Debug)
                .AddFilter("System", LogLevel.Debug)
                .AddSimpleConsole(c => c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss]");
        });

        private const int DEFAULT_PORT = 3307;

        static async Task Main(string[] args)
        {
            RegisterDecorator();
            var port = GetPort(args);
            InternalLoggerFactory.DefaultFactory = _loggerFactory;
            var serivces = new ServiceCollection();
            serivces.AddSingleton<IConfiguration>(serviceProvider => _configuration);
            serivces.AddSingleton<ILoggerFactory>(serviceProvider => _loggerFactory);
            serivces.AddSingleton<ShardingProxyOption>(serviceProvider =>
            {
                var proxyOption = serviceProvider.GetRequiredService<IOptionsSnapshot<ShardingProxyOption>>().Value;
                if (port.HasValue)
                {
                    proxyOption.Port = port.Value;
                }

                return proxyOption;
            });
            // serivces.AddSingleton<ServerHandlerInitializer>();
            serivces.AddSingleton<PackDecoder>();
            serivces.AddSingleton<PackEncoder>();
            // serivces.AddSingleton<ApplicationChannelInboundHandler>();
            serivces.AddSingleton<IShardingProxy,ShardingProxy>();
            serivces.Configure<ShardingProxyOption>(_configuration);
            var buildServiceProvider = serivces.BuildServiceProvider();
            var shardingProxyOption = buildServiceProvider.GetRequiredService<ShardingProxyOption>();
            var authentication = new Authentication();
            authentication.Users.Add("root",new ProxyUser("123456",new List<string>(){"test"}));
            ShardingProxyContext.GetInstance().Init(authentication,new Dictionary<string, string>());
            await StartAsync(buildServiceProvider,shardingProxyOption, GetPort(args));
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        private static void RegisterDecorator()
        {
            NewInstanceServiceLoader.Register<IRouteDecorator>();
            NewInstanceServiceLoader.Register<ISqlRewriteContextDecorator>();
            NewInstanceServiceLoader.Register<IResultProcessEngine>();
        }

        private static int? GetPort(string[] args)
        {
            if (args.Length == 0)
            {
                return null;
            }

            return int.TryParse(args[0], out var port) ? port : null;
        }

        private static async Task StartAsync(IServiceProvider serviceProvider,ShardingProxyOption option, int? port)
        {
            var shardingProxy = serviceProvider.GetRequiredService<IShardingProxy>();
           await shardingProxy.StartAsync();
        }
    }
}