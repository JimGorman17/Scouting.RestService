using System;

namespace Scouting.DataLayer
{
    public class AuthTokenRepository : Repository<AuthToken>
    {
        public string GetGoogleIdByAuthToken(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                throw new ArgumentNullException("authToken");
            }

            return Database.SingleOrDefault<string>("SELECT GoogleID FROM AuthTokens WHERE (Token = @0)", authToken);
        }
    }
}
