using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using LambdaWithApiGateway.DockerFunction.Model;
using LambdaWithApiGateway.DockerFunction.Repository;

namespace LambdaWithApiGateway.DockerFunction.Service;

public class UserService : IUserService
{
    private readonly IResponseService _responseService;
    private readonly IUserRepository _userRepository;

    public UserService(IResponseService responseService, IUserRepository userRepository)
    {
        _responseService = responseService;
        _userRepository = userRepository;
    }

    public async Task<APIGatewayProxyResponse> GetUserAsync(APIGatewayProxyRequest request, string tableName)
    {
        try
        {
            if (request.PathParameters.TryGetValue("userId", out string? userId))
            {
                User user = await _userRepository.GetUserForIdAsync(userId, tableName);
                return _responseService.GenerateSuccessResponse(user);
            }
            return _responseService.GenerateBadRequestResponse("User ID is required");
        }
        catch (Exception exception)
        {
            return _responseService.GenerateInternalServerErrorResponse(exception);
        }
    }

    public async Task<APIGatewayProxyResponse> AddUserAsync(APIGatewayProxyRequest request, string tableName)
    {
        try
        {
            User? newUser = JsonSerializer.Deserialize<User>(request.Body);
            if (newUser != null && newUser.Id != null && newUser.Name != null)
            {
                User createdUser = await _userRepository.AddUserAsync(newUser, tableName);
                return _responseService.GenerateSuccessResponse(createdUser);
            }
            return _responseService.GenerateBadRequestResponse("User ID and name are required in the body");
        }
        catch (Exception exception)
        {
            return _responseService.GenerateInternalServerErrorResponse(exception);
        }
    }

    public async Task<APIGatewayProxyResponse> DeleteUserAsync(APIGatewayProxyRequest request, string tableName)
    {
        try
        {
            if (request.PathParameters.TryGetValue("userId", out var userId))
            {
                User user = await _userRepository.DeleteUserAsync(userId, tableName);
                return _responseService.GenerateSuccessResponse(user);
            }
            return _responseService.GenerateBadRequestResponse("User ID is required");
        }
        catch (Exception exception)
        {
            return _responseService.GenerateInternalServerErrorResponse(exception);
        }
    }
}