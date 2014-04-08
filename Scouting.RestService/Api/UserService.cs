using System;
using System.Net;
using AutoMapper;
using Newtonsoft.Json;
using Scouting.DataLayer;
using Scouting.RestService.Dtos;
using ServiceStack;

namespace Scouting.RestService.Api
{
    public class UserService : Service
    {
        public UserRepository UserRepository { get; set; }
        public AuthTokenRepository AuthTokenRepository { get; set; }

        [Route("/User/UpdateFavoriteTeam")]
        public class UserUpdateFavoriteTeamRequest
        {
            public string AuthToken { get; set; }
            public int TeamId { get; set; }
        }

        public object Post(UserUpdateFavoriteTeamRequest request)
        {
            var googleId = GetGoogleId(request.AuthToken, AuthTokenRepository, UserRepository);

            if (request.TeamId <= 0)
            {
                throw new ArgumentOutOfRangeException("FavoriteTeamId");
            }

            var existingUser = UserRepository.GetUserByGoogleId(googleId);
            
            existingUser.FavoriteTeamId = request.TeamId;
            UserRepository.Update(existingUser);

            return new HttpStatusResult(HttpStatusCode.OK);
        }

        public static string GetGoogleId(string authToken, AuthTokenRepository authTokenRepository, UserRepository userRepository)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                throw new ArgumentNullException("authToken");
            }

            User user;
            var googleId = authTokenRepository.GetGoogleIdByAuthToken(authToken);
            if (googleId == null)
            {
                user = RecordTheAuthTokenInTheDatabase(authToken, authTokenRepository);
                googleId = user.GoogleId;
            }
            else
            {
                user = userRepository.GetUserByGoogleId(googleId);
                if (user == null)
                {
                    var googlePlusLoginDto = GetGooglePlusLoginDto(authToken);
                    if (googlePlusLoginDto == null || String.IsNullOrEmpty(googlePlusLoginDto.id))
                    {
                        throw new ArgumentException(String.Format("Unable to retrieve a Google user for auth token: '{0}'.", authToken));
                    }

                    user = Mapper.Map<GooglePlusLoginDto, User>(googlePlusLoginDto);
                }
            }
            AddOrUpdateTheUser(user, userRepository);
            return googleId;
        }

        private static User RecordTheAuthTokenInTheDatabase(string authToken, AuthTokenRepository authTokenRepository)
        {
            var googlePlusLoginDto = GetGooglePlusLoginDto(authToken);
            if (googlePlusLoginDto == null || String.IsNullOrEmpty(googlePlusLoginDto.id))
            {
                throw new ArgumentException(String.Format("Unable to retrieve a Google user for auth token: '{0}'.", authToken));
            }

            var mappedUser = Mapper.Map<GooglePlusLoginDto, User>(googlePlusLoginDto);
            authTokenRepository.Add(new AuthToken
            {
                Token = authToken,
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

        private static void AddOrUpdateTheUser(User user, UserRepository userRepository)
        {
            var existingUser = userRepository.GetUserByGoogleId(user.GoogleId);
            if (existingUser == null)
            {
                user.CreateDate = DateTimeOffset.Now;
                userRepository.Add(user);
            }
            else
            {
                if (existingUser.DisplayName != user.DisplayName ||
                    existingUser.GivenName != user.GivenName ||
                    existingUser.FamilyName != user.FamilyName ||
                    existingUser.Link != user.Link ||
                    existingUser.Picture != user.Picture ||
                    existingUser.Gender != user.Gender ||
                    existingUser.Locale != user.Locale)
                {
                    existingUser.DisplayName = user.DisplayName;
                    existingUser.GivenName = user.GivenName;
                    existingUser.FamilyName = user.FamilyName;
                    existingUser.Link = user.Link;
                    existingUser.Picture = user.Picture;
                    existingUser.Gender = user.Gender;
                    existingUser.Locale = user.Locale;
                    existingUser.UpdateDate = DateTimeOffset.Now;
                    userRepository.Update(existingUser);
                }
            }
        }
    }
}