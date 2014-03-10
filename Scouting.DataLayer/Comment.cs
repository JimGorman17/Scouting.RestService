using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    [TableName("Comments")]
    [PrimaryKey("CommentId")]
    public class Comment : IEntity
    {
        public int CommentId { get; set; }
        public int PlayerId { get; set; }
        public int GoogleId { get; set; }
        [Column("Comment")]
        public string CommentString { get; set; }
        public bool Deleted { get; set; }

        [Ignore]
        public bool IsNew
        {
            get
            {
                return CommentId == default(int);
            }
        }
    }
}
