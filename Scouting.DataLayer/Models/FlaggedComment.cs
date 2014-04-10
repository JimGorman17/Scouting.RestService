using System;

namespace Scouting.DataLayer.Models
{
    [TableName("FlaggedComments")]
    [PrimaryKey("FlaggedCommentId")]
    public class FlaggedComment : IEntity
    {
        public int FlaggedCommentId { get; set; }
        public int CommentId { get; set; }
        public string GoogleId { get; set; }
        public DateTimeOffset FlaggedDate { get; set; }
        public bool Handled { get; set; }

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
