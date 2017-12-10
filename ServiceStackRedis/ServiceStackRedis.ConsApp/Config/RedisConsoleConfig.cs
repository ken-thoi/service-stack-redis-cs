using System;

namespace ServiceStackRedis.ConsApp.Config
{
    public static class RedisConsoleConfig
    {
        static RedisConsoleConfig()
        {
            //LogManager.LogFactory = new InMemoryLogFactory();
        }

        public static bool IgnoreLongTests = true;

        public static string SingleHost => Environment.GetEnvironmentVariable("CI_REDIS") ?? "localhost";

        public static string GeoHost => Environment.GetEnvironmentVariable("CI_REDIS") ?? "10.0.0.121";

        public static readonly string[] MasterHosts = new[] { "localhost" };
        public static readonly string[] SlaveHosts = new[] { "localhost" };

        public const int RedisPort = 6379;

        public static string SingleHostConnectionString => SingleHost + ":" + RedisPort;

        //public static BasicRedisClientManager BasicClientManger => new BasicRedisClientManager(new[] { SingleHostConnectionString });
    }
}
