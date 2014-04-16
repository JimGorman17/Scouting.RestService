using System;
using Scouting.DataLayer.Helpers;

namespace Scouting.DataLayer.Models
{
    public class CommentUserRow
    {
        public string PictureUrl { get; set; }
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

        public string UserInfo
        {
            get { return String.Format("{0}<br><small>Favorite Team: {1}<br>Last Post: {2}</small>", DisplayName, FavoriteTeam, LastPostDateString); }
        }
    }
}
