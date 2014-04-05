using System;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    [TableName("AuthTokens")]
    [PrimaryKey("AuthTokenId")]
    public class AuthToken : IEntity
    {
        public int AuthTokenId { get; set; }
        public string Token { get; set; }
        public string GoogleId { get; set; }
        public DateTimeOffset CreateDate { get; set; }

        [Ignore]
        public bool IsNew
        {
            get
            {
                return AuthTokenId == default(int);
            }
        }
    }
}
