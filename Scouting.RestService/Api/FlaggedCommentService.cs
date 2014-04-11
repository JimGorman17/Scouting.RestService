using System.Net;
using Scouting.DataLayer;
using Scouting.DataLayer.Repositories;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Scouting.RestService.Api
{
    public class FlaggedCommentService : Service
    {
        public FlaggedCommentRepository FlaggedCommentRepository { get; set; }
        public AuthTokenRepository AuthTokenRepository { get; set; }
        public UserRepository UserRepository { get; set; }

        [Route("/FlaggedComment/GetCommentsForModerator")]
        public class GetCommentsForModerator
        {
            public string AuthToken { get; set; }
        }

        public object Post(GetCommentsForModerator request)
        {
            User user;
            try
            {
                var googleId = UserService.GetGoogleId(request.AuthToken, AuthTokenRepository, UserRepository);
                user = UserRepository.GetUserByGoogleId(googleId);
            }
            catch
            {
                return new HttpStatusResult(HttpStatusCode.Unauthorized);
            }

            if (user.IsAdmin == false)
            {
                return new HttpStatusResult(HttpStatusCode.Forbidden);
            }

            return FlaggedCommentRepository.GetFlaggedCommentsForModerator();
        }
    }
}