using System.Collections.Generic;

namespace ServiceStackRedis.ConsApp.Entities
{
    public class User
    {
        public User()
        {
            BlogIds = new List<long>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public List<long> BlogIds { get; set; }
    }
}
