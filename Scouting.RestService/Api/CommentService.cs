using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Scouting.DataLayer;
using Scouting.DataLayer.Models;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class CommentService : Service
    {
        public Database Database { get; set; }
        public CommentRepository CommentRepository { get; set; }
        public AuthTokenRepository AuthTokenRepository { get; set; }
        public UserRepository UserRepository { get; set; }

        [Route("/Comment/GetAllByPlayerId")]
        public class CommentGetAllByPlayerIdRequest
        {
            public int PlayerId { get; set; }
        }

        public class CommentGetAllByPlayerIdResponse
        {
            public List<CommentView> Comments { get; set; }
        }

        public object Get(CommentGetAllByPlayerIdRequest commentRequest)
        {
            var comments = CommentRepository.GetAllByPlayerId(commentRequest.PlayerId);

            return new CommentGetAllByPlayerIdResponse { Comments = comments };
        }

        [Route("/Comment/Create")]
        public class CommentCreateRequest
        {
            public string AuthToken { get; set; }
            public int PlayerId { get; set; }
            public string CommentString { get; set; }
        }

        public object Post(CommentCreateRequest request)
        {
            var googleId = UserService.GetGoogleId(request.AuthToken, AuthTokenRepository, UserRepository);

            CommentRepository.Add(new Comment
                {
                    PlayerId = request.PlayerId,
                    GoogleId = googleId,
                    CommentString = request.CommentString.Trim(),
                    CreateDate = DateTimeOffset.Now
                });

            return new HttpStatusResult(HttpStatusCode.OK);
        }

        [Route("/Comment/GetTotalsByTeam")]
        public class CommentGetTotalsByTeamRequest
        {
        }

        public class TeamCommentRow
        {
            public string Team { get; set; }
            public int Count { get; set; }
        }

        public object Get(CommentGetTotalsByTeamRequest request)
        {
            var results = Database.Query<TeamCommentRow>("SELECT T.Location + ' ' + T.Nickname AS [Team], COUNT(C.CommentID) AS [Count]" +
                                        "FROM Comments C " +
                                        "INNER JOIN Players P " +
                                        "ON	(C.PlayerID = P.PlayerID) " +
                                        "INNER JOIN Teams T " +
                                        "ON	(P.Team = T.Abbreviation) " +
                                        "GROUP BY T.Location, T.Nickname " +
                                        "ORDER BY COUNT(C.CommentID) DESC");

            return results;
        }

        [Route("/Comment/GetTotalsByUser")]
        public class CommentGetTotalsByUserRequest
        {
            public int NumberOfUsers { get; set; }
        }

        public class CommentUserRow
        {
            public string Picture { get; set; }
            public string DisplayName { get; set; }
            public string FavoriteTeam { get; set; }
            public int Count { get; set; }
        }

        public object Get(CommentGetTotalsByUserRequest request)
        {
            var results = Database.Query<CommentUserRow>("SELECT U.Picture, U.DisplayName, T.Location + ' ' + T.Nickname AS [FavoriteTeam], COUNT(C.CommentID) AS [Count] " +
                                                   "FROM Comments C " +
                                                   "INNER JOIN Users U " +
                                                   "ON	(C.GoogleID = U.GoogleID) " +
                                                   "LEFT OUTER JOIN Teams T " +
                                                   "ON (U.FavoriteTeamID = T.TeamID)" +
                                                   "GROUP BY U.Picture, U.DisplayName, T.Location, T.Nickname " +
                                                   "ORDER BY COUNT(C.CommentID) DESC");

            return results.Take(request.NumberOfUsers);
        }
    }
}