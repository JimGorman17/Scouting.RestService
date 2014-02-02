using System.Collections.Generic;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class PlayerService : Service
    {
        public object Get(PlayerGetAllRequest request)
        {
            var players = new Repository<Player>().GetAll(); // TODO: Get Repository<T> through IOC.

            return new PlayerGetAllResponse { Players = players };
        }

        [Route("/Player/GetAll")]
        public class PlayerGetAllRequest
        {
        }

        public class PlayerGetAllResponse
        {
            public List<Player> Players { get; set; }
        }
    }
}