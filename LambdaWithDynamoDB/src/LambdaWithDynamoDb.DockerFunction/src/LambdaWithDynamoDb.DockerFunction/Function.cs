using Amazon.Lambda.Core;
using LambdaWithDynamoDb.DockerFunction.Model;
using LambdaWithDynamoDb.DockerFunction.Repository;
using LambdaWithDynamoDb.DockerFunction.Service;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaWithDynamoDb.DockerFunction;

public class Function
{
    private readonly IQueryService _queryService = new QueryService(new DynamoDbUserRepository());
    
    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<User> FunctionHandler(Input input, ILambdaContext context)
    {
        string tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME") ?? throw new InvalidOperationException();
        return await _queryService.QueryUserTableAsync(input.Id, tableName);
    }
}