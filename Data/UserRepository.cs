namespace DotnetAPI.Data
{
    public class UserRepository
    {
        DataContextEF _entityFramework;

        public UserRepository(IConfiguration configuration)
        {
            _entityFramework = new DataContextEF(configuration);
        }
    }

}