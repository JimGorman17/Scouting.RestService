using System.Collections.Generic;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class PlayerService : Service
    {
        public PlayerRepository PlayerRepository { get; set; }

        public object Get(PlayerGetAllByTeamIdRequest playerRequest)
        {
            var players = PlayerRepository.GetAllByTeamId(playerRequest.TeamId);

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