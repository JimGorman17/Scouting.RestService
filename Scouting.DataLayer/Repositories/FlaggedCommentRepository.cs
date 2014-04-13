using System.Collections.Generic;
using System.Linq;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer.Repositories
{
    public class FlaggedCommentRepository : Repository<FlaggedComment>
    {
        public List<FlaggedComment> GetUnhandledCommentsByFlaggedCommentByCommentIdAndGoogleId(int commentId, string googleId)
        {
            return Database.Query<FlaggedComment>("WHERE (Handled = @0) AND (CommentID = @1) AND (GoogleID = @2)", false, commentId, googleId).ToList();
        }

        public List<FlaggedCommentForModerator> GetFlaggedCommentsForModerator()
        {
            var results = Database.Query<FlaggedCommentForModerator>(
                "SELECT C.CommentID, C.Comment, RTRIM(LTRIM(P.FirstName + ' ' + P.LastName)) AS [PlayerName], RTRIM(LTRIM(T.Location + ' ' + T.Nickname)) AS [TeamName], COALESCE(C.UpdateDate, C.CreateDate) AS [UpdateDate], COUNT(C.CommentID) AS [NumberOfFlags] " +
                "FROM FlaggedComments FC " +
                "INNER JOIN Comments C " +
                "ON	(FC.CommentID = C.CommentID) AND " +
                "(C.Deleted = 0) " +
                "INNER JOIN Players P " +
                "ON (C.PlayerID = P.PlayerID) " +
                "INNER JOIN Teams T " + 
                "ON (P.Team = T.Abbreviation) " +
                "INNER JOIN Users U " +
                "ON	(FC.GoogleID = U.GoogleID) " +
                "WHERE (FC.Handled = 0) " +
                "GROUP BY C.CommentID, C.Comment, RTRIM(LTRIM(P.FirstName + ' ' + P.LastName)), RTRIM(LTRIM(T.Location + ' ' + T.Nickname)), COALESCE(C.UpdateDate, C.CreateDate) " +
                "ORDER BY COUNT(C.CommentID) DESC, COALESCE(C.UpdateDate, C.CreateDate) DESC");

            return results.ToList();
        }
    }
}
