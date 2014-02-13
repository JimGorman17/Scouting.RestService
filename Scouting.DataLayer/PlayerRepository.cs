using System.Collections.Generic;
using System.Linq;

namespace Scouting.DataLayer
{
    public class PlayerRepository : Repository<Player>
    {
        public List<Player> GetAllByTeamAbbreviation(string teamAbbreviation)
        {
            return _db.Query<Player>("WHERE Team = @0", teamAbbreviation).ToList();
        }
    }
}
