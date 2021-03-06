﻿using System.Collections.Generic;
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
                "SELECT T.TeamID, T.Location + ' ' + T.Nickname AS [Team], MAX(COALESCE(C.UpdateDate, C.CreateDate)) AS [LastPostDate], COUNT(C.CommentID) AS [Count] " +
                "FROM Teams T " +
                "INNER JOIN Players P " +
                "ON	(T.Abbreviation = P.Team) " +
                "LEFT OUTER JOIN Comments C " +
                "ON	(P.PlayerID = C.PlayerID) AND (C.Deleted = 0) " +
                "GROUP BY T.TeamID, T.Location, T.Nickname " +
                "ORDER BY COUNT(C.CommentID) DESC, T.Location, T.Nickname");

            return results.ToList();
        }

        public List<CommentUserRow> GetTotalsByUser(int numberOfUsers)
        {
            var results = Database.Query<CommentUserRow>(
                "SELECT U.Picture AS [PictureUrl], U.DisplayName, T.Location + ' ' + T.Nickname AS [FavoriteTeam], MAX(COALESCE(C.UpdateDate, C.CreateDate)) AS [LastPostDate], COUNT(C.CommentID) AS [Count] " +
                "FROM Comments C " +
                "INNER JOIN Users U " +
                "ON	(C.GoogleID = U.GoogleID) " +
                "LEFT OUTER JOIN Teams T " +
                "ON (U.FavoriteTeamID = T.TeamID) " +
                "WHERE (C.Deleted = 0) " +
                "GROUP BY U.Picture, U.DisplayName, T.Location, T.Nickname " +
                "ORDER BY COUNT(C.CommentID) DESC");

            return results.Take(numberOfUsers).ToList();
        }
    }
}
