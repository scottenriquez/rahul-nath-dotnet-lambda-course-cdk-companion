using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaWithApiGateway.Tests.Unit;

public class FunctionExecutionRoleTests
{
    [Fact]
    public void Stack_FunctionExecutionPolicy_ShouldOnlyHaveAccessToOneDynamoDbResourceAndAllCloudWatchLogs()
    {
        // arrange
        App app = new App();
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

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
                                    "Action", new []
                                    {
                                        "dynamodb:Query",
                                        "dynamodb:DescribeTable",
                                        "dynamodb:DeleteItem",
                                        "dynamodb:PutItem" 
                                    }
                                },
                                {
                                    "Resource", new Dictionary<string, object>()
                                    {
                                        {
                                            "Fn::GetAtt", Match.AnyValue()
                                        }
                                    }
                                }
                            },
                            new Dictionary<string, object>()
                            {
                                {
                                    "Action", new []
                                    {
                                        "logs:CreateLogGroup",
                                        "logs:CreateLogStream",
                                        "logs:PutLogEvents"
                                    }
                                },
                                {
                                    "Resource", "*" 
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
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

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