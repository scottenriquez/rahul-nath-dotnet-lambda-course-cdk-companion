using LambdaWithDynamoDb.DockerFunction.Model;
using LambdaWithDynamoDb.DockerFunction.Repository;

namespace LambdaWithDynamoDb.DockerFunction.Service;

public class QueryService : IQueryService
{
    private IUserRepository _userRepository;

    public QueryService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> QueryUserTableAsync(string userGuid, string tableName)
    {
        return await _userRepository.GetUserForIdAsync(userGuid, tableName);
    }
}