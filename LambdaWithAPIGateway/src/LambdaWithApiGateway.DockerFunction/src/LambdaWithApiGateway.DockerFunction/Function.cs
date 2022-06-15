using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using LambdaWithApiGateway.DockerFunction.Repository;
using LambdaWithApiGateway.DockerFunction.Service;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace LambdaWithApiGateway.DockerFunction;

public class Function
{
    private readonly IUserService _userService = new UserService(new ResponseService(), new DynamoDbUserRepository());
    
    /// <summary>
    /// A Lambda function for handling API Gateway requests for GET, POST, and DELETE for users in a DynamoDB table 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        string tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME") ?? throw new InvalidOperationException();
        return request.RequestContext.HttpMethod switch
        {
            "GET" => await _userService.GetUserAsync(request, tableName),
            "POST" => await _userService.AddUserAsync(request, tableName),
            "DELETE" => await _userService.DeleteUserAsync(request, tableName),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}