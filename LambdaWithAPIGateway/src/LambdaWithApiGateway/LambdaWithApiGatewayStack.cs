using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Resource = Amazon.CDK.AWS.APIGateway.Resource;

namespace LambdaWithApiGateway
{
    public class LambdaWithApiGatewayStack : Stack
    {
        internal LambdaWithApiGatewayStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            DockerImageCode dockerImageCode = DockerImageCode.FromImageAsset("src/LambdaWithApiGateway.DockerFunction/src/LambdaWithApiGateway.DockerFunction");
            DockerImageFunction dockerImageFunction = new DockerImageFunction(this, "ApiGatewayLambdaFunction",
                new DockerImageFunctionProps()
                {
                    Architecture = Architecture.ARM_64,
                    Code = dockerImageCode,
                    Description = ".NET 6 Docker Lambda function for querying DynamoDB",
                    Timeout = Duration.Minutes(5) 
                }
            );
            LambdaRestApi api = new LambdaRestApi(this, "ApiGateway", new LambdaRestApiProps {
                Handler = dockerImageFunction,
                Proxy = false
            });
            // GET /users
            Resource usersResource = api.Root.AddResource("users");
            usersResource.AddMethod("GET");
            // GET /users/{userId}
            Resource userResource = usersResource.AddResource("{userId}");
            userResource.AddMethod("GET");
        }
    }
}
