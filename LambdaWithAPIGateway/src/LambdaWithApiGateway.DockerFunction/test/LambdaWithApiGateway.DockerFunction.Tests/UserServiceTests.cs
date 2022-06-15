using Amazon.Lambda.APIGatewayEvents;
using FakeItEasy;
using FluentAssertions;
using LambdaWithApiGateway.DockerFunction.Model;
using LambdaWithApiGateway.DockerFunction.Repository;
using LambdaWithApiGateway.DockerFunction.Service;
using Xunit;

namespace LambdaWithApiGateway.DockerFunction.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task Should_GetUserAsync_ForValidInput()
    {
        // arrange
        string tableName = "UserTable";
        User user = new User()
        {
            Id = "e6f0ca6d-ba1c-4eec-9e2f-672e8f92447f",
            Name = "Item Name"
        };
        string userSerializedJson = $"{{\"Id\":\"{user.Id}\",\"Name\":\"{user.Name}\"}}";
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = userSerializedJson 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest()
        {
            PathParameters = new Dictionary<string, string>()
            {
                {
                    "userId", user.Id
                }
            }
        };
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateSuccessResponse(user)).Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        A.CallTo(() => userRepository.GetUserForIdAsync(user.Id, tableName)).Returns(user);
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.GetUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldReturnBadRequest_GetUserAsync_ForInvalidInput()
    {
        // arrange
        string tableName = "UserTable";
        string badRequestMessage = "User ID is required";
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 400,
            Body = badRequestMessage 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest()
        {
            PathParameters = new Dictionary<string, string>()
        };
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateBadRequestResponse(badRequestMessage)).Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.GetUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldReturnInternalServerError_GetUserAsync_ForInvalidInput()
    {
        // arrange
        string tableName = "UserTable";
        Exception exception = new Exception("An error occurred.");
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 500,
            Body = exception.Message 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest();
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateInternalServerErrorResponse(exception)).WithAnyArguments().Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.GetUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task Should_AddUserAsync_ForValidInput()
    {
        // arrange
        string tableName = "UserTable";
        User user = new User()
        {
            Id = "e6f0ca6d-ba1c-4eec-9e2f-672e8f92447f",
            Name = "Item Name"
        };
        string userSerializedJson = $"{{\"Id\":\"{user.Id}\",\"Name\":\"{user.Name}\"}}";
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = userSerializedJson
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest()
        {
            Body = userSerializedJson 
        };
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateSuccessResponse(user)).Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        A.CallTo(() => userRepository.AddUserAsync(user, tableName)).WithAnyArguments().Returns(user);
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.AddUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldReturnBadRequest_AddUserAsync_ForInvalidInput()
    {
        // arrange
        string tableName = "UserTable";
        string badRequestMessage = "User ID and name are required in the body";
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 400,
            Body = badRequestMessage 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest()
        {
            Body = "{\"NotId\": 1234}"
        };
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateBadRequestResponse(badRequestMessage)).Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.AddUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldReturnInternalServerError_AddUserAsync_ForInvalidInput()
    {
        // arrange
        string tableName = "UserTable";
        Exception exception = new Exception("An error occurred.");
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 500,
            Body = exception.Message 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest();
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateInternalServerErrorResponse(exception)).WithAnyArguments().Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.AddUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task Should_DeleteUserAsync_ForValidInput()
    {
        // arrange
        string tableName = "UserTable";
        User user = new User()
        {
            Id = "e6f0ca6d-ba1c-4eec-9e2f-672e8f92447f",
            Name = "Item Name"
        };
        string userSerializedJson = $"{{\"Id\":\"{user.Id}\",\"Name\":\"{user.Name}\"}}";
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = userSerializedJson 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest()
        {
            PathParameters = new Dictionary<string, string>()
            {
                {
                    "userId", user.Id
                }
            }
        };
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateSuccessResponse(user)).Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        A.CallTo(() => userRepository.DeleteUserAsync(user.Id, tableName)).Returns(user);
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.DeleteUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldReturnBadRequest_DeleteUserAsync_ForInvalidInput()
    {
        // arrange
        string tableName = "UserTable";
        string badRequestMessage = "User ID is required";
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 400,
            Body = badRequestMessage 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest()
        {
            PathParameters = new Dictionary<string, string>()
        };
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateBadRequestResponse(badRequestMessage)).Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.DeleteUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldReturnInternalServerError_DeleteUserAsync_ForInvalidInput()
    {
        // arrange
        string tableName = "UserTable";
        Exception exception = new Exception("An error occurred.");
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 500,
            Body = exception.Message 
        };
        APIGatewayProxyRequest request = new APIGatewayProxyRequest();
        IResponseService responseService = A.Fake<IResponseService>();
        A.CallTo(() => responseService.GenerateInternalServerErrorResponse(exception)).WithAnyArguments().Returns(expectedResponse);
        IUserRepository userRepository = A.Fake<IUserRepository>();
        IUserService userService = new UserService(responseService, userRepository);

        // act
        APIGatewayProxyResponse response = await userService.DeleteUserAsync(request, tableName);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
}