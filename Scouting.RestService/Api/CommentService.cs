using System;
using System.Collections.Generic;
using System.Configuration;
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
            public string AuthToken { get; set; }
            public int PlayerId { get; set; }
        }

        public class CommentGetAllByPlayerIdResponse
        {
            public List<CommentView> Comments { get; set; }
        }

        public object Post(CommentGetAllByPlayerIdRequest request)
        {
            var comments = CommentRepository.GetAllByPlayerId(request.PlayerId).OrderByDescending(c => c.CreateDate).ToList();
            SetCanEditOrDeleteProperty(request, comments);

            return new CommentGetAllByPlayerIdResponse { Comments = comments };
        }

        private void SetCanEditOrDeleteProperty(CommentGetAllByPlayerIdRequest request, IEnumerable<CommentView> comments)
        {
            int editOrDeleteTolerance;
            var editOrDeleteToleranceValueExists =
                int.TryParse(ConfigurationManager.AppSettings[ApplicationSettingsKeys.EditOrDeleteToleranceInMinutes],
                             out editOrDeleteTolerance);
            if (editOrDeleteToleranceValueExists == false)
            {
                throw new ConfigurationErrorsException(String.Format("{0} does not exist in web.config",
                                                                     ApplicationSettingsKeys.EditOrDeleteToleranceInMinutes));
            }

            if (String.IsNullOrEmpty(request.AuthToken) == false)
            {
                try
                {
                    var googleId = UserService.GetGoogleId(request.AuthToken, AuthTokenRepository, UserRepository);
                    var user = UserRepository.GetUserByGoogleId(googleId);
                    foreach (var commentView in comments)
                    {
                        if (user.IsAdmin ||
                            (googleId.Equals(commentView.GoogleId, StringComparison.OrdinalIgnoreCase) &&
                             (commentView.UpdateDate.HasValue
                                  ? DateTimeOffset.Now.Subtract(commentView.UpdateDate.Value).TotalMinutes
                                  : DateTimeOffset.Now.Subtract(commentView.CreateDate).TotalMinutes) < editOrDeleteTolerance
                            )
                            )
                        {
                            commentView.CanEditOrDelete = true;
                        }
                    }
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                }
            }
        }

        [Route("/Comment/Save")]
        public class CommentSaveRequest
        {
            public int CommentId { get; set; }
            public string AuthToken { get; set; }
            public int PlayerId { get; set; }
            public string CommentString { get; set; }
        }

        public object Post(CommentSaveRequest request)
        {
            var googleId = UserService.GetGoogleId(request.AuthToken, AuthTokenRepository, UserRepository);

            if (request.CommentId == 0)
            {
                CommentRepository.Add(new Comment
                    {
                        CommentId = request.CommentId,
                        PlayerId = request.PlayerId,
                        GoogleId = googleId,
                        CommentString = request.CommentString.Trim(),
                        CreateDate = DateTimeOffset.Now,
                    });
            }
            else
            {
                var comment = CommentRepository.GetByCommentIdAndGoogleId(request.CommentId, googleId);
                if (comment == null)
                {
                    throw new UnauthorizedAccessException(
                        String.Format("Google Id '{0}' is not allowed to modify Comment Id '{1}'.", googleId,
                                      request.CommentId));
                }

                comment.CommentString = request.CommentString.Trim();
                comment.UpdateDate = DateTimeOffset.Now;
                CommentRepository.Update(comment);
            }

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