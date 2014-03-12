namespace Scouting.DataLayer
{
    public class AuthTokenRepository : Repository<AuthToken>
    {
        public string GetGoogleIdByAuthToken(string authToken)
        {
            return Db.SingleOrDefault<string>("SELECT GoogleID FROM AuthTokens WHERE (Token = @0)", authToken);
        }
    }
}
