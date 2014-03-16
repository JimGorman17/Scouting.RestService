using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Newtonsoft.Json;
using Scouting.DataLayer;
using Scouting.RestService.Dtos;
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

        [Route("/Comment/Create")]
        public class CommentCreateRequest
        {
            public string AuthToken { get; set; }
            public int PlayerId { get; set; }
            public string CommentString { get; set; }
        }

        public object Get(CommentGetAllByPlayerIdRequest commentRequest)
        {
            var comments = new CommentRepository().GetAllByPlayerId(commentRequest.PlayerId); // TODO: Get Repository<T> through IOC.

            return new CommentGetAllByPlayerIdResponse { Comments = comments };
        }

        public object Post(CommentCreateRequest request)
        {
            var authTokenRepository = new AuthTokenRepository(); // TODO: Get Repository<T> through IOC.
            var googleId = authTokenRepository.GetGoogleIdByAuthToken(request.AuthToken);
            if (googleId == null)
            {
                var mappedUser = RecordTheAuthTokenInTheDatabase(request, authTokenRepository);
                googleId = mappedUser.GoogleId;
                AddOrUpdateTheUser(mappedUser);
            }

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

        private static User RecordTheAuthTokenInTheDatabase(CommentCreateRequest request, IRepository<AuthToken> authTokenRepository)
        {
            var googlePlusLoginDto = GetGooglePlusLoginDto(request.AuthToken);
            if (googlePlusLoginDto == null || String.IsNullOrEmpty(googlePlusLoginDto.id))
            {
                throw new ArgumentException(String.Format("Unable to retrieve a Google user for auth token: '{0}'.", request.AuthToken));
            }

            var mappedUser = Mapper.Map<GooglePlusLoginDto, User>(googlePlusLoginDto);
            authTokenRepository.Add(new AuthToken
            {
                Token = request.AuthToken,
                GoogleId = mappedUser.GoogleId,
                CreateDate = DateTimeOffset.Now
            });
            return mappedUser;
        }

        private static GooglePlusLoginDto GetGooglePlusLoginDto(string authToken)
        {
            if (String.IsNullOrEmpty(authToken))
            {
                throw new ArgumentNullException(authToken);
            }

            var authTokenToGoogleIdUrl = System.Configuration.ConfigurationManager.AppSettings[ApplicationSettingsKeys.AuthTokenToGoogleIdUrl];
            if (String.IsNullOrEmpty(authTokenToGoogleIdUrl))
            {
                throw new InvalidOperationException("AuthTokenToGoogleIdUrl must be specified in the configuration file.");
            }

            string jsonData;
            using (var webClient = new WebClient())
            {
                jsonData = webClient.DownloadString(String.Format(authTokenToGoogleIdUrl, authToken));
            }

            return String.IsNullOrEmpty(jsonData) ? new GooglePlusLoginDto() : JsonConvert.DeserializeObject<GooglePlusLoginDto>(jsonData);
        }

        private static void AddOrUpdateTheUser(User mappedUser)
        {
            var userRepository = new UserRepository(); // TODO: Get Repository<T> through IOC.
            var existingUser = userRepository.GetUserByGoogleId(mappedUser.GoogleId);
            if (existingUser == null)
            {
                mappedUser.CreateDate = DateTimeOffset.Now;
                userRepository.Add(mappedUser);
            }
            else
            {
                if (existingUser.DisplayName != mappedUser.DisplayName ||
                    existingUser.GivenName != mappedUser.GivenName ||
                    existingUser.FamilyName != mappedUser.FamilyName ||
                    existingUser.Link != mappedUser.Link ||
                    existingUser.Picture != mappedUser.Picture ||
                    existingUser.Gender != mappedUser.Gender ||
                    existingUser.Locale != mappedUser.Locale)
                {
                    existingUser.DisplayName = mappedUser.DisplayName;
                    existingUser.GivenName = mappedUser.GivenName;
                    existingUser.FamilyName = mappedUser.FamilyName;
                    existingUser.Link = mappedUser.Link;
                    existingUser.Picture = mappedUser.Picture;
                    existingUser.Gender = mappedUser.Gender;
                    existingUser.Locale = mappedUser.Locale;
                    existingUser.UpdateDate = DateTimeOffset.Now;
                    userRepository.Update(existingUser);
                }
            }
        }
    }
}