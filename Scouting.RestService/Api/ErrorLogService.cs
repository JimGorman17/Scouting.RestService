using System;
using System.Net;
using Scouting.DataLayer;
using Scouting.DataLayer.Models;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class ErrorLogService : Service
    {
        public Repository<ErrorLog> ErrorLogRepository { get; set; }
        public AuthTokenRepository AuthTokenRepository { get; set; }
        public UserRepository UserRepository { get; set; }

        [Route("/ErrorLog/Create")]
        public class ErrorLogCreateRequest
        {
            public string Application { get; set; }
            public string PhoneId { get; set; }
            public string ErrorMessage { get; set; }
            public string StackTrace { get; set; }
            public string AuthToken { get; set; }
        }

        public object Post(ErrorLogCreateRequest request)
        {
            string googleId = null;
            User existingUser = new User();

            if (String.IsNullOrEmpty(request.AuthToken) == false)
            {
                try
                {
                    googleId = UserService.GetGoogleId(request.AuthToken, AuthTokenRepository, UserRepository);
                    existingUser = UserRepository.GetUserByGoogleId(googleId);
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                }
            }

            ErrorLogRepository.Add(new ErrorLog
            {
                Application = request.Application,
                PhoneId = request.PhoneId,
                Message = request.ErrorMessage,
                StackTrace = request.StackTrace,
                CreateDate = DateTimeOffset.Now,
                UserId = 0 < existingUser.UserId ? existingUser.UserId : (int?) null,
                GoogleId = googleId
            });

            return new HttpStatusResult(HttpStatusCode.OK);
        }
    }
}