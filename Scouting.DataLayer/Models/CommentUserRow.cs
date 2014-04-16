using System;
using Scouting.DataLayer.Helpers;

namespace Scouting.DataLayer.Models
{
    public class CommentUserRow
    {
        public string Picture { get; set; }
        public string DisplayName { get; set; }
        public string FavoriteTeam { get; set; }
        public int Count { get; set; }
        public DateTimeOffset LastPostDate { get; set; }
        public string LastPostDateString
        {
            get
            {
                return DateTimeOffset.Now.Subtract(LastPostDate).ToReadableString(true);
            }
        }
    }
}
