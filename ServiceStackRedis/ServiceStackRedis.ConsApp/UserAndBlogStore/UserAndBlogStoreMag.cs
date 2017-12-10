using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStackRedis.ConsApp.Config;
using ServiceStackRedis.ConsApp.Entities;

namespace ServiceStackRedis.ConsApp.UserAndBlogStore
{
    public static class UserAndBlogStoreMag
    {
        static readonly RedisClient Redis = new RedisClient(RedisConsoleConfig.SingleHost);

        public static void OnBeforeEachRun()
        {
            // Reset all keys
            Redis.FlushAll();
        }

        public static void StoreAndRetrieveSomeBlogs()
        {
            //Retrieve strongly-typed Redis clients that let's you natively persist POCO's
            var redisUsers = Redis.As<User>();
            var redisBlogs = Redis.As<Blog>();
            //Create the user, getting a unique User Id from the User sequence.
            var mythz = new User { Id = redisUsers.GetNextSequence(), Name = "Demis Bellot" };

            //create some blogs using unique Ids from the Blog sequence. Also adding references
            var mythzBlogs = new List<Blog>
            {
                new Blog
                {
                    Id = redisBlogs.GetNextSequence(),
                    UserId = mythz.Id,
                    UserName = mythz.Name,
                    Tags = new List<string> { "Architecture", ".NET", "Redis" },
                },
                new Blog
                {
                    Id = redisBlogs.GetNextSequence(),
                    UserId = mythz.Id,
                    UserName = mythz.Name,
                    Tags = new List<string> { "Music", "Twitter", "Life" },
                },
            };
            //Add the blog references
            mythzBlogs.ForEach(x => mythz.BlogIds.Add(x.Id));

            //Store the user and their blogs
            redisUsers.Store(mythz);
            redisBlogs.StoreAll(mythzBlogs);

            //retrieve all blogs
            var blogs = redisBlogs.GetAll();

            //Recursively print the values of the POCO (For T.Dump() Extension method see: http://mono.servicestack.net/mythz_blog/?p=202)
            blogs.PrintDump();
        }
    }
}
