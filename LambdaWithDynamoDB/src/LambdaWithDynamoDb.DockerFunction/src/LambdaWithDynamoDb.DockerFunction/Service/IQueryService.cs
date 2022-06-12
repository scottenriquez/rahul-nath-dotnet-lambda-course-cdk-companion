using LambdaWithDynamoDb.DockerFunction.Model;

namespace LambdaWithDynamoDb.DockerFunction.Service;

public interface IQueryService
{
    public Task<User> QueryUserTableAsync(string userGuid, string tableName);
}