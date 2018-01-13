using System.Collections.Generic;

namespace HouseCrawler.Core.Models
{
    public class Size
    {
        /// <summary>
        /// 
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int height { get; set; }
    }

    public class PhotosItem
    {
        /// <summary>
        /// 
        /// </summary>
        public Size size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string alt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string layout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string topic_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string seq_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string author_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creation_date { get; set; }
    }

    public class Author
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_suicide { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string alt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string large_avatar { get; set; }
    }

    public class TopicsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string updated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PhotosItem> photos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int like_count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string alt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string locked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Author author { get; set; }
        /// <summary>
        /// 这次我们是认真的 投资中港白金公寓
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string share_url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int comments_count { get; set; }
    }

    public class DoubanTopic
    {
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int start { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TopicsItem> topics { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
    }
}
