using Amazon.Lambda.APIGatewayEvents;

namespace LambdaWithApiGateway.DockerFunction.Service;

public interface IUserService
{
    public Task<APIGatewayProxyResponse> GetUserAsync(APIGatewayProxyRequest request, string tableName);
    public Task<APIGatewayProxyResponse> AddUserAsync(APIGatewayProxyRequest request, string tableName);
    public Task<APIGatewayProxyResponse> DeleteUserAsync(APIGatewayProxyRequest request, string tableName);
}