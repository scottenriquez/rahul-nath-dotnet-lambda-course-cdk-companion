using Amazon.Lambda.APIGatewayEvents;

namespace LambdaWithApiGateway.DockerFunction.Service;

public interface IResponseService
{
    APIGatewayProxyResponse GenerateSuccessResponse(object data);
    APIGatewayProxyResponse GenerateBadRequestResponse(string message);
    APIGatewayProxyResponse GenerateInternalServerErrorResponse(Exception exception);
}