using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    [TableName("Players")]
    [PrimaryKey("PlayerId")]
    public class Player : IEntity
    {
        public int PlayerId { get; set; }
        public string Position { get; set; }
        public string Number { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        [Column("Team")]
        public string TeamAbbrevation { get; set; }
        
        [Ignore]
        public bool IsNew
        {
            get
            {
                return PlayerId == default(int);
            }
        }
    }
}
