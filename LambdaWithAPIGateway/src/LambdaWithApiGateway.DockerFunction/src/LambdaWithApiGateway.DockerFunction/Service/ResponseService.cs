using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace LambdaWithApiGateway.DockerFunction.Service;

public class ResponseService : IResponseService
{
    public APIGatewayProxyResponse GenerateSuccessResponse(object data)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(data) 
        };
    }
    
    public APIGatewayProxyResponse GenerateBadRequestResponse(string message)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = message 
        };
    }

    public APIGatewayProxyResponse GenerateInternalServerErrorResponse(Exception exception)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 500,
            Body = exception.Message
        };
    }
}