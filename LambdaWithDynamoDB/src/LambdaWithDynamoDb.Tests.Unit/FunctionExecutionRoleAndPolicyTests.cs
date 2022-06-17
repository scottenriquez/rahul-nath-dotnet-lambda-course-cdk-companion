using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaWithDynamoDb.Tests.Unit;

public class FunctionExecutionRoleTests
{
    [Fact]
    public void Stack_FunctionExecutionPolicy_ShouldOnlyHaveAccessToOneResource()
    {
        // arrange
        App app = new App();
        LambdaWithDynamoDbStack stack = new LambdaWithDynamoDbStack(app, "LambdaWithDynamoDbStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::IAM::ManagedPolicy", new Dictionary<string, object>()
        {
            {
                "PolicyDocument", new Dictionary<string, object>()
                {
                    {
                        "Statement", new []
                        {
                            new Dictionary<string, object>()
                            {
                                {
                                    "Resource", new Dictionary<string, object>()
                                    {
                                        {
                                            "Fn::GetAtt", Match.AnyValue()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }
    
    [Fact]
    public void Stack_FunctionExecutionRole_ShouldAllowLambdaServicePrincipal()
    {
        // arrange
        App app = new App();
        LambdaWithDynamoDbStack stack = new LambdaWithDynamoDbStack(app, "LambdaWithDynamoDbStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::IAM::Role", new Dictionary<string, object>()
        {
            {
                "AssumeRolePolicyDocument", new Dictionary<string, object>()
                {
                    {
                        "Statement", new []
                        {
                            new Dictionary<string, object>()
                            {
                                {
                                    "Principal", new Dictionary<string, string>()
                                    {
                                        {
                                            "Service", "lambda.amazonaws.com"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }
}