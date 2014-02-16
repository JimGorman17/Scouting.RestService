using System.Collections.Generic;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class PlayerService : Service
    {
        public object Get(PlayerGetAllByTeamIdRequest playerRequest)
        {
            var players = new PlayerRepository().GetAllByTeamId(playerRequest.TeamId); // TODO: Get Repository<T> through IOC.

            return new PlayerGetAllByTeamAbbreviationResponse { Players = players };
        }

        [Route("/Player/GetAllByTeamId")]
        public class PlayerGetAllByTeamIdRequest
        {
            public int TeamId { get; set; }
        }

        public class PlayerGetAllByTeamAbbreviationResponse
        {
            public List<Player> Players { get; set; }
        }
    }
}