using System.Collections.Generic;
using System.Linq;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    public class CommentRepository : Repository<Comment>
    {
        public Comment GetByCommentId(int commentId)
        {
            return Database.SingleOrDefault<Comment>("WHERE (CommentID = @0)", commentId);
        }

        public List<CommentView> GetAllByPlayerId(int playerId)
        {
            return Database.Query<CommentView>("SELECT C.*, U.UserID, U.DisplayName, U.Picture FROM Comments C INNER JOIN Users U ON (C.GoogleID = U.GoogleID) WHERE (C.PlayerID = @0) AND (C.Deleted = @1)", playerId, false).ToList();
        }

        public List<TeamCommentRow> GetTotalsByTeam()
        {
            var results = Database.Query<TeamCommentRow>(
                "SELECT T.Location + ' ' + T.Nickname AS [Team], COUNT(C.CommentID) AS [Count]" +
                "FROM Comments C " +
                "INNER JOIN Players P " +
                "ON	(C.PlayerID = P.PlayerID) " +
                "INNER JOIN Teams T " +
                "ON	(P.Team = T.Abbreviation) " +
                "GROUP BY T.Location, T.Nickname " +
                "ORDER BY COUNT(C.CommentID) DESC");

            return results.ToList();
        }

        public List<CommentUserRow> GetTotalsByUser(int numberOfUsers)
        {
            var results = Database.Query<CommentUserRow>(
                "SELECT U.Picture, U.DisplayName, T.Location + ' ' + T.Nickname AS [FavoriteTeam], COUNT(C.CommentID) AS [Count] " +
                "FROM Comments C " +
                "INNER JOIN Users U " +
                "ON	(C.GoogleID = U.GoogleID) " +
                "LEFT OUTER JOIN Teams T " +
                "ON (U.FavoriteTeamID = T.TeamID)" +
                "GROUP BY U.Picture, U.DisplayName, T.Location, T.Nickname " +
                "ORDER BY COUNT(C.CommentID) DESC");

            return results.Take(numberOfUsers).ToList();
        }
    }
}
