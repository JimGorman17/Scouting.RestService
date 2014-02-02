using System.Collections.Generic;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class TeamService : Service
    {
        public object Get(TeamGetAllRequest request)
        {
            var teams = new Repository<Team>().GetAll(); // TODO: Get Repository<T> through IOC.

            return new TeamGetAllResponse { Teams = teams };
        }

        [Route("/Team/GetAll")]
        public class TeamGetAllRequest
        {
        }

        public class TeamGetAllResponse
        {
            public List<Team> Teams { get; set; }
        }
    }
}