using System;
using System.Collections.Generic;
using System.Net;
using Scouting.DataLayer;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class CommentService : Service
    {
        [Route("/Comment/GetAllByPlayerId")]
        public class CommentGetAllByPlayerIdRequest
        {
            public int PlayerId { get; set; }
        }

        public class CommentGetAllByPlayerIdResponse
        {
            public List<CommentView> Comments { get; set; }
        }

        public object Get(CommentGetAllByPlayerIdRequest commentRequest)
        {
            var comments = new CommentRepository().GetAllByPlayerId(commentRequest.PlayerId); // TODO: Get Repository<T> through IOC.

            return new CommentGetAllByPlayerIdResponse { Comments = comments };
        }

        [Route("/Comment/Create")]
        public class CommentCreateRequest
        {
            public string AuthToken { get; set; }
            public int PlayerId { get; set; }
            public string CommentString { get; set; }
        }

        public object Post(CommentCreateRequest request)
        {
            var googleId = UserService.GetGoogleId(request.AuthToken);

            var commentRepository = new CommentRepository(); // TODO: Get Repository<T> through IOC.
            commentRepository.Add(new Comment
                {
                    PlayerId = request.PlayerId,
                    GoogleId = googleId,
                    CommentString = request.CommentString.Trim(),
                    CreateDate = DateTimeOffset.Now
                });

            return new HttpStatusResult(HttpStatusCode.OK);
        }
    }
}