using LambdaWithApiGateway.DockerFunction.Model;

namespace LambdaWithApiGateway.DockerFunction.Repository;

public interface IUserRepository
{
    Task<User> GetUserForIdAsync(string userGuid, string tableName);
    Task<User> AddUserAsync(User user, string tableName);
    Task<User> DeleteUserAsync(string userGuid, string tableName);
}