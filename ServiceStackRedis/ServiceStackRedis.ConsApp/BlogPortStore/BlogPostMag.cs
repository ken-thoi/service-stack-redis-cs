using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStackRedis.ConsApp.Config;
using ServiceStackRedis.ConsApp.Entities;

namespace ServiceStackRedis.ConsApp.BlogPortStore
{
    public static class BlogPostMag
    {
        static readonly RedisClient Redis = new RedisClient(RedisConsoleConfig.SingleHost);

        public static void OnBeforeEachRun()
        {
            // Reset all keys
            Redis.FlushAll();
            InsertTestData();
        }

        public static void InsertTestData()
        {
            var redisUsers = Redis.As<User>();
            var redisBlogs = Redis.As<Blog>();
            var redisBlogPosts = Redis.As<BlogPost>();

            var ayende = new User { Id = redisUsers.GetNextSequence(), Name = "Oren Eini" };
            var mythz = new User { Id = redisUsers.GetNextSequence(), Name = "Demis Bellot" };

            var ayendeBlog = new Blog
            {
                Id = redisBlogs.GetNextSequence(),
                UserId = ayende.Id,
                UserName = ayende.Name,
                Tags = new List<string> { "Architecture", ".NET", "Databases" },
            };

            var mythzBlog = new Blog
            {
                Id = redisBlogs.GetNextSequence(),
                UserId = mythz.Id,
                UserName = mythz.Name,
                Tags = new List<string> { "Architecture", ".NET", "Databases" },
            };

            var blogPosts = new List<BlogPost>
            {
                new BlogPost
                {
                    Id = redisBlogPosts.GetNextSequence(),
                    BlogId = ayendeBlog.Id,
                    Title = "RavenDB",
                    Categories = new List<string> { "NoSQL", "DocumentDB" },
                    Tags = new List<string> {"Raven", "NoSQL", "JSON", ".NET"} ,
                    Comments = new List<BlogPostComment>
                    {
                        new BlogPostComment { Content = "First Comment!", CreatedDate = DateTime.UtcNow,},
                        new BlogPostComment { Content = "Second Comment!", CreatedDate = DateTime.UtcNow,},
                    }
                },
                new BlogPost
                {
                    Id = redisBlogPosts.GetNextSequence(),
                    BlogId = mythzBlog.Id,
                    Title = "Redis",
                    Categories = new List<string> { "NoSQL", "Cache" },
                    Tags = new List<string> {"Redis", "NoSQL", "Scalability", "Performance"},
                    Comments = new List<BlogPostComment>
                    {
                        new BlogPostComment { Content = "First Comment!", CreatedDate = DateTime.UtcNow,}
                    }
                },
                new BlogPost
                {
                    Id = redisBlogPosts.GetNextSequence(),
                    BlogId = ayendeBlog.Id,
                    Title = "Cassandra",
                    Categories = new List<string> { "NoSQL", "Cluster" },
                    Tags = new List<string> {"Cassandra", "NoSQL", "Scalability", "Hashing"},
                    Comments = new List<BlogPostComment>
                    {
                        new BlogPostComment { Content = "First Comment!", CreatedDate = DateTime.UtcNow,}
                    }
                },
                new BlogPost
                {
                    Id = redisBlogPosts.GetNextSequence(),
                    BlogId = mythzBlog.Id,
                    Title = "Couch Db",
                    Categories = new List<string> { "NoSQL", "DocumentDB" },
                    Tags = new List<string> {"CouchDb", "NoSQL", "JSON"},
                    Comments = new List<BlogPostComment>
                    {
                        new BlogPostComment {Content = "First Comment!", CreatedDate = DateTime.UtcNow,}
                    }
                },
            };

            ayende.BlogIds.Add(ayendeBlog.Id);
            ayendeBlog.BlogPostIds.AddRange(blogPosts.Where(x => x.BlogId == ayendeBlog.Id).Map(x => x.Id));

            mythz.BlogIds.Add(mythzBlog.Id);
            mythzBlog.BlogPostIds.AddRange(blogPosts.Where(x => x.BlogId == mythzBlog.Id).Map(x => x.Id));

            redisUsers.Store(ayende);
            redisUsers.Store(mythz);
            redisBlogs.StoreAll(new[] { ayendeBlog, mythzBlog });
            redisBlogPosts.StoreAll(blogPosts);
        }

        public static void ShowListOfBlogs()
        {
            var redisBlogs = Redis.As<Blog>();
            var blogs = redisBlogs.GetAll();
            blogs.PrintDump();
        }

        public static void ShowListOfRecentPostsAndComments()
        {
            //Get strongly-typed clients
            var redisBlogPosts = Redis.As<BlogPost>();
            var redisComments = Redis.As<BlogPostComment>();
            {
                //To keep this example let's pretend this is a new list of blog posts
                var newIncomingBlogPosts = redisBlogPosts.GetAll();

                //Let's get back an IList<BlogPost> wrapper around a Redis server-side List.
                var recentPosts = redisBlogPosts.Lists["urn:BlogPost:RecentPosts"];
                var recentComments = redisComments.Lists["urn:BlogPostComment:RecentComments"];

                foreach (var newBlogPost in newIncomingBlogPosts)
                {
                    //Prepend the new blog posts to the start of the 'RecentPosts' list
                    recentPosts.Prepend(newBlogPost);

                    //Prepend all the new blog post comments to the start of the 'RecentComments' list
                    newBlogPost.Comments.ForEach(recentComments.Prepend);
                }

                //Make this a Rolling list by only keep the latest 3 posts and comments
                recentPosts.Trim(0, 2);
                recentComments.Trim(0, 2);

                //Print out the last 3 posts:
                recentPosts.GetAll().PrintDump();
            }
        }

        public static void ShowATagCloud()
        {
            //Get strongly-typed clients
            var redisBlogPosts = Redis.As<BlogPost>();
            var newIncomingBlogPosts = redisBlogPosts.GetAll();

            foreach (var newBlogPost in newIncomingBlogPosts)
            {
                //For every tag in each new blog post, increment the number of times each Tag has occurred 
                newBlogPost.Tags.ForEach(x =>
                    Redis.IncrementItemInSortedSet("urn:TagCloud", x, 1));
            }

            //Show top 5 most popular tags with their scores
            var tagCloud = Redis.GetRangeWithScoresFromSortedSetDesc("urn:TagCloud", 0, 4);
            tagCloud.PrintDump();
        }

        public static void ShowAllCategories()
        {
            var redisBlogPosts = Redis.As<BlogPost>();
            var blogPosts = redisBlogPosts.GetAll();

            foreach (var blogPost in blogPosts)
            {
                blogPost.Categories.ForEach(x =>
                      Redis.AddItemToSet("urn:Categories", x));
            }

            var uniqueCategories = Redis.GetAllItemsFromSet("urn:Categories");
            uniqueCategories.PrintDump();
        }

        public static void ShowPostAndAllComments()
        {
            //There is nothing special required here as since comments are Key Value Objects 
            //they are stored and retrieved with the post
            var postId = 1;
            var redisBlogPosts = Redis.As<BlogPost>();
            var selectedBlogPost = redisBlogPosts.GetById(postId.ToString());

            selectedBlogPost.PrintDump();
        }

        public static void AddCommentToExistingPost()
        {
            var postId = 1;
            var redisBlogPosts = Redis.As<BlogPost>();
            var blogPost = redisBlogPosts.GetById(postId.ToString());
            blogPost.Comments.Add(
                new BlogPostComment { Content = "Third Post!", CreatedDate = DateTime.UtcNow });
            redisBlogPosts.Store(blogPost);

            var refreshBlogPost = redisBlogPosts.GetById(postId.ToString());
            refreshBlogPost.PrintDump();
        }

        public static void ShowAllPostsForTheDocumentDbCategory()
        {
            var redisBlogPosts = Redis.As<BlogPost>();
            var newIncomingBlogPosts = redisBlogPosts.GetAll();

            foreach (var newBlogPost in newIncomingBlogPosts)
            {
                //For each post add it's Id into each of it's 'Cateogry > Posts' index
                newBlogPost.Categories.ForEach(x =>
                        Redis.AddItemToSet("urn:Category:" + x, newBlogPost.Id.ToString()));
            }

            //Retrieve all the post ids for the category you want to view
            var documentDbPostIds = Redis.GetAllItemsFromSet("urn:Category:DocumentDB");

            //Make a batch call to retrieve all the posts containing the matching ids 
            //(i.e. the DocumentDB Category posts)
            var documentDbPosts = redisBlogPosts.GetByIds(documentDbPostIds);

            documentDbPosts.PrintDump();
        }
    }
}
