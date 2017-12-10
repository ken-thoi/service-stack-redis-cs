using System;
using ServiceStack.Redis;

namespace ServiceStackRedis.ConsApp.Config
{
    //Ssl bool If this is an SSL connection
    //Db  int The Redis DB this connection should be set to
    //Client  string A text alias to specify for this connection for analytic purposes
    //Password    string UrlEncoded version of the Password for this connection
    //ConnectTimeout  int Timeout in ms for making a TCP Socket connection
    //SendTimeout int Timeout in ms for making a synchronous TCP Socket Send
    //ReceiveTimeout  int Timeout in ms for waiting for a synchronous TCP Socket Receive
    //IdleTimeOutSecs int Timeout in Seconds for an Idle connection to be considered active
    //NamespacePrefix string Use a custom prefix for ServiceStack.Redis internal index colletions

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

        public static BasicRedisClientManager BasicClientManger => new BasicRedisClientManager(new[] { SingleHostConnectionString });
    }
}
