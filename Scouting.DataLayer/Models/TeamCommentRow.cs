using System;
using Scouting.DataLayer.Helpers;

namespace Scouting.DataLayer.Models
{
    public class TeamCommentRow
    {
        public int TeamId { get; set; }
        public string Team { get; set; }
        public DateTimeOffset? LastPostDate { get; set; }
        public int Count { get; set; }
        public string LastPostDateString
        {
            get
            {
                return LastPostDate.HasValue == false ? String.Empty : DateTimeOffset.Now.Subtract(LastPostDate.Value).ToReadableString(true);
            }
        }
    }
}
