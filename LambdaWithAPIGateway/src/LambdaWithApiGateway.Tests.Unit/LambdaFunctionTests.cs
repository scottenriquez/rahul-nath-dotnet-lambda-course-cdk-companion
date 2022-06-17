using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaWithApiGateway.Tests.Unit;

public class LambdaFunctionTests
{
    [Fact]
    public void Stack_LambdaFunction_ShouldHaveDynamoDbTableEnvironmentVariable()
    {
        // arrange
        App app = new App();
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::Lambda::Function", new Dictionary<string, object>()
        {
            {
                "Environment", new Dictionary<string, object>()
                {
                    {
                        "Variables", new Dictionary<string, object>()
                        {
                            {
                                "DYNAMODB_TABLE_NAME", new Dictionary<string, object>()
                                {
                                    {
                                        "Ref", Match.AnyValue()
                                    }
                                }
                            }
                        } 
                    }
                }
            },
            {
                "PackageType", "Image" 
            }
        });
    }
}