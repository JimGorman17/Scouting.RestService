using System;
using Scouting.DataLayer.Helpers;

namespace Scouting.DataLayer.Models
{
    public class FlaggedCommentForModerator
    {
        public int CommentId { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public string LastUpdated
        {
            get
            {
                return DateTimeOffset.Now.Subtract(UpdateDate).ToReadableString(true);
            }
        }
        public int Count { get; set; }
    }
}
