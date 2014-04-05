using System;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    [TableName("Comments")]
    [PrimaryKey("CommentId")]
    public class Comment : IEntity
    {
        public int CommentId { get; set; }
        public int PlayerId { get; set; }
        public string GoogleId { get; set; }
        [Column("Comment")]
        public string CommentString { get; set; }
        public bool Deleted { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        [Ignore]
        public bool IsNew
        {
            get
            {
                return CommentId == default(int);
            }
        }
    }

    public class CommentView : Comment
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Picture { get; set; }
    }
}
