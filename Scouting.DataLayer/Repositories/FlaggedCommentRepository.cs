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
    }
}
