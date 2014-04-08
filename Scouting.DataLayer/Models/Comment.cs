using System;
using System.Configuration;
using Scouting.DataLayer.Helpers;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    [TableName("Comments")]
    [PrimaryKey("CommentId")]
    public class Comment : IEntity
    {
        public int CommentId { get; set; }
        public int PlayerId { get; set; }
        public string GoogleId { get; set; }
        [Column("Comment")]
        public string CommentString { get; set; }
        public bool Deleted { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        [Ignore]
        public bool IsNew
        {
            get
            {
                return CommentId == default(int);
            }
        }
    }

    public class CommentView : Comment
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Picture { get; set; }
        public string ViewingUsersGoogleId { get; set; }
        public bool CanEditOrDelete
        {
            get
            {
                return
                    String.IsNullOrEmpty(ViewingUsersGoogleId) == false &&
                    ViewingUsersGoogleId.Trim().Equals(GoogleId.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    (UpdateDate.HasValue ? DateTimeOffset.Now.Subtract(UpdateDate.Value).TotalMinutes : DateTimeOffset.Now.Subtract(CreateDate).TotalMinutes) < int.Parse(ConfigurationManager.AppSettings[ApplicationSettingsKeys.EditOrDeleteToleranceInMinutes]);
            }
        }
        public string FormattedComment
        {
            get { return String.Format("{0} - <b><small>{1}</small></b>", CommentString.Trim(), UpdateDate.HasValue ? DateTimeOffset.Now.Subtract(UpdateDate.Value).ToReadableString(true) : DateTimeOffset.Now.Subtract(CreateDate).ToReadableString(true)); }
        }
    }
}
