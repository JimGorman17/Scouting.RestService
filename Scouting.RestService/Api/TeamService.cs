using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Types;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class TeamService : Service
    {
        [Route("/Team/GetAll")]
        public class TeamGetAllRequest
        {
        }

        public class TeamGetAllResponse
        {
            public List<Team> Teams { get; set; }
        }

        public object Get(TeamGetAllRequest request)
        {
            var teams = new Repository<Team>().GetAll(); // TODO: Get Repository<T> through IOC.

            return new TeamGetAllResponse { Teams = teams };
        }

        [Route("/Team/GetClosestTeamRequest")]
        public class TeamGetClosestTeamRequest
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public class TeamGetClosestTeamResponse
        {
            public Team Team { get; set; }
        }

        public object Get(TeamGetClosestTeamRequest request)
        {
            var givenPoint = SqlGeography.Point(request.Latitude, request.Longitude, 4326);
            var allTeams = new Repository<Team>().GetAll(); // TODO: Get Repository<T> through IOC.

            var nearest = allTeams.OrderBy(t => t.CenterPoint.STDistance(givenPoint)).First();

            return new TeamGetClosestTeamResponse { Team = nearest };
        }

    }
}