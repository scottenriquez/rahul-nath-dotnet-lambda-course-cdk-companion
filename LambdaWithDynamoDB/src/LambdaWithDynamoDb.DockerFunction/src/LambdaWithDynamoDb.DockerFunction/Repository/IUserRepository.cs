using LambdaWithDynamoDb.DockerFunction.Model;

namespace LambdaWithDynamoDb.DockerFunction.Repository;

public interface IUserRepository
{
    Task<User> GetUserForIdAsync(string userGuid, string tableName);
}