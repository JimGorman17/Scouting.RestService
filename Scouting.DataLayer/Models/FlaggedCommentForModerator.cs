using System;
using Scouting.DataLayer.Helpers;

namespace Scouting.DataLayer.Models
{
    public class FlaggedCommentForModerator
    {
        public int CommentId { get; set; }
        public string Comment { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public string LastUpdated
        {
            get
            {
                return DateTimeOffset.Now.Subtract(UpdateDate).ToReadableString(true);
            }
        }
        public string FormattedComment
        {
            get { return String.Format("{0}<br><b><small>{1}, {2}, {3}</small></b>", Comment.Trim(), PlayerName, TeamName, LastUpdated); }
        }
        public int NumberOfFlags { get; set; }
    }
}
