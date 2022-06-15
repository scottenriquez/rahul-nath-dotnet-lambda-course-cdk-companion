using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using LambdaWithApiGateway.DockerFunction.Model;
using LambdaWithApiGateway.DockerFunction.Service;
using Xunit;

namespace LambdaWithApiGateway.DockerFunction.Tests;

public class ResponseServiceTests
{
    [Fact]
    public void Should_GenerateSuccessResponse_ForValidInput()
    {
        // arrange
        User user = new User()
        {
            Id = "e6f0ca6d-ba1c-4eec-9e2f-672e8f92447f",
            Name = "Item Name"
        };
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 200,
            Body = "{\"Id\":\"e6f0ca6d-ba1c-4eec-9e2f-672e8f92447f\",\"Name\":\"Item Name\"}"
        };
        IResponseService responseService = new ResponseService();

        // act
        APIGatewayProxyResponse response = responseService.GenerateSuccessResponse(user);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public void Should_GenerateBadRequestResponse_ForValidInput()
    {
        // arrange
        string message = "User ID is required";
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 400,
            Body = message
        };
        IResponseService responseService = new ResponseService();

        // act
        APIGatewayProxyResponse response = responseService.GenerateBadRequestResponse(message);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public void Should_GenerateInternalServerErrorResponse_ForValidInput()
    {
        // arrange
        Exception exception = new Exception("Insufficient IAM permissions");
        APIGatewayProxyResponse expectedResponse = new APIGatewayProxyResponse()
        {
            StatusCode = 500,
            Body = exception.Message 
        };
        IResponseService responseService = new ResponseService();

        // act
        APIGatewayProxyResponse response = responseService.GenerateInternalServerErrorResponse(exception);

        // assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
}