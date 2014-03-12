using System;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    [TableName("Users")]
    [PrimaryKey("UserId")]
    public class User : IEntity
    {
        public int UserId { get; set; }
        public string GoogleId { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Link { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; }
        public string Locale { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        [Ignore]
        public bool IsNew
        {
            get
            {
                return UserId == default(int);
            }
        }
    }
}
