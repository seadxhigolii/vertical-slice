﻿namespace vertical_slice.Entities
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content{ get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? PublishedOnUtc { get; set; }
    }
}
