using System.Collections.Generic;
using System.Linq;

namespace Scouting.DataLayer
{
    public class PlayerRepository : Repository<Player>
    {
        public List<Player> GetAllByTeamId(int teamId)
        {
            return Db.Query<Player>("SELECT P.* FROM Teams T INNER JOIN Players P ON (T.Abbreviation = P.Team) WHERE (T.TeamID = @0)", teamId).ToList();
        }
    }
}
