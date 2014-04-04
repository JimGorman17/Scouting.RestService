namespace Scouting.DataLayer
{
    public class UserRepository : Repository<User>
    {
        public User GetUserByGoogleId(string googleId)
        {
            return Database.SingleOrDefault<User>("WHERE (GoogleID = @0)", googleId);
        }
    }
}
