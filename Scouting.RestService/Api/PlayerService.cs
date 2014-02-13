using System.Collections.Generic;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class PlayerService : Service
    {
        public object Get(PlayerGetAllByTeamAbbreviationRequest playerRequest)
        {
            var players = new PlayerRepository().GetAllByTeamAbbreviation(playerRequest.TeamAbbreviation); // TODO: Get Repository<T> through IOC.

            return new PlayerGetAllByTeamAbbreviationResponse { Players = players };
        }

        [Route("/Player/GetAllByTeamAbbreviationRequest")]
        public class PlayerGetAllByTeamAbbreviationRequest
        {
            public string TeamAbbreviation { get; set; }
        }

        public class PlayerGetAllByTeamAbbreviationResponse
        {
            public List<Player> Players { get; set; }
        }
    }
}