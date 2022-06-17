using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaWithApiGateway.Tests.Unit;

public class ApiGatewayTests
{
    [Fact]
    public void Stack_ApiGateway_ShouldHaveUserResource()
    {
        // arrange
        App app = new App();
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::ApiGateway::Resource", new Dictionary<string, string>()
        {
            {
                "PathPart", "users"
            }
        });
    }
    
    [Fact]
    public void Stack_ApiGateway_ShouldHaveUserIdResource()
    {
        // arrange
        App app = new App();
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::ApiGateway::Resource", new Dictionary<string, string>()
        {
            {
                "PathPart", "{userId}"
            }
        });
    }
}