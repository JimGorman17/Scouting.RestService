using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    [TableName("Teams")]
    [PrimaryKey("TeamId")]
    public class Team : IEntity
    {
        public int TeamId { get; set; }
        public string Location { get; set; }
        public string Nickname { get; set; }
        public string Abbreviation { get; set; }
        public string Conference { get; set; }
        public string Division { get; set; }
        public SqlGeography CenterPoint { get; protected set; }

        [Ignore]
        public bool IsNew
        {
            get
            {
                return TeamId == default(int);
            }
        }
    }
}
