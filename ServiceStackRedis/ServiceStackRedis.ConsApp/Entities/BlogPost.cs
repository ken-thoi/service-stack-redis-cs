using System.Collections.Generic;

namespace ServiceStackRedis.ConsApp.Entities
{
    public class BlogPost
    {
        public BlogPost()
        {
            Categories = new List<string>();
            Tags = new List<string>();
            Comments = new List<BlogPostComment>();
        }

        public long Id { get; set; }
        public long BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public List<BlogPostComment> Comments { get; set; }
    }
}
