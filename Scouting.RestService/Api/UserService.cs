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
        [Route("/User/UpdateFavoriteTeam")]
        public class UserUpdateFavoriteTeamRequest
        {
            public string AuthToken { get; set; }
            public int TeamId { get; set; }
        }

        public object Post(UserUpdateFavoriteTeamRequest request)
        {
            var googleId = GetGoogleId(request.AuthToken);

            if (request.TeamId <= 0)
            {
                throw new ArgumentOutOfRangeException("FavoriteTeamId");
            }

            var userRepository = new UserRepository(); // TODO: Get Repository<T> through IOC.
            var existingUser = userRepository.GetUserByGoogleId(googleId);
            
            existingUser.FavoriteTeamId = request.TeamId;
            userRepository.Update(existingUser);

            return new HttpStatusResult(HttpStatusCode.OK);
        }

        public static string GetGoogleId(string authToken)
        {
            var authTokenRepository = new AuthTokenRepository(); // TODO: Get Repository<T> through IOC.
            var googleId = authTokenRepository.GetGoogleIdByAuthToken(authToken);
            if (googleId == null)
            {
                var user = RecordTheAuthTokenInTheDatabase(authToken, authTokenRepository);
                googleId = user.GoogleId;
                AddOrUpdateTheUser(user);
            }
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

        private static void AddOrUpdateTheUser(User user)
        {
            var userRepository = new UserRepository(); // TODO: Get Repository<T> through IOC.
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