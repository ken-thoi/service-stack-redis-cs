using System;

namespace ServiceStackRedis.ConsApp.Entities
{
    public class BlogPostComment
    {
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
