using System;
using System.Net;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class CommentService : Service
    {
        public object Post(CommentCreateRequest request)
        {
            string googleId;

            var authTokenRepository = new AuthTokenRepository(); // TODO: Get Repository<T> through IOC.
            if (authTokenRepository.GetGoogleIdByAuthToken(request.AuthToken) == null)
            {
                // TODO: Implement.

                //authTokenRepository.Add(new AuthToken
                //    {
                //        Token = request.AuthToken,
                //        GoogleId = ,
                //        CreateDate = DateTimeOffset.Now
                //    });
            }
            var commentRepository = new Repository<Comment>(); // TODO: Get Repository<T> through IOC.
            // TODO: Convert request.AuthToken to GoogleId.

            commentRepository.Add(new Comment
                {
                    CommentString = request.CommentString,
                    GoogleId = googleId,
                    PlayerId = request.PlayerId
                });

            return new HttpStatusResult(HttpStatusCode.OK);
        }

        [Route("/Comment/Create")]
        public class CommentCreateRequest
        {
            public string AuthToken { get; set; }
            public int PlayerId { get; set; }
            public string CommentString { get; set; }
        }
    }
}