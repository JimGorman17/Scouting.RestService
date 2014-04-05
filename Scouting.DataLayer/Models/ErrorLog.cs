using System;

namespace Scouting.DataLayer.Models
{
    [TableName("ErrorLogs")]
    [PrimaryKey("ErrorLogId")]
    public class ErrorLog : IEntity
    {
        public int ErrorLogId { get; set; }
        public string Application { get; set; }
        public string PhoneId { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime? OccurredDate { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public int? UserId { get; set; }
        public string GoogleId { get; set; }

        [Ignore]
        public bool IsNew
        {
            get
            {
                return ErrorLogId == default(int);
            }
        }
    }
}
