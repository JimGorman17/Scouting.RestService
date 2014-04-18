using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Types;
using Scouting.DataLayer;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Scouting.RestService.Api
{
    public class TeamService : Service
    {
        public Repository<Team> TeamRepository { get; set; }

        [Route("/Team/GetAll")]
        public class TeamGetAllRequest
        {
        }

        public object Get(TeamGetAllRequest request)
        {
            return TeamRepository.GetAll();
        }

        [Route("/Team/GetClosestTeam")]
        public class TeamGetClosestTeamRequest
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
        
        public object Get(TeamGetClosestTeamRequest request)
        {
            var givenPoint = SqlGeography.Point(request.Latitude, request.Longitude, 4326);
            var allTeams = TeamRepository.GetAll();

            return allTeams.OrderBy(t => t.CenterPoint.STDistance(givenPoint))
                .ThenBy(t => Guid.NewGuid()) // Randomize so that we can give the NY Giants and the NY Jets both an equal chance.
                .First();
        }
    }
}