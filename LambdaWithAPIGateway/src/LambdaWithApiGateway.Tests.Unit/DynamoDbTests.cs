using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaWithApiGateway.Tests.Unit;

public class DynamoDbTests
{
    [Fact]
    public void Stack_DynamoDb_ShouldHaveFiveRcuAndWcu()
    {
        // arrange
        App app = new App();
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::DynamoDB::Table", new Dictionary<string, object>()
        {
            {
                "ProvisionedThroughput", new Dictionary<string, int>()
                {
                    {
                        "ReadCapacityUnits", 5
                    },
                    {
                        "WriteCapacityUnits", 5
                    }
                }
            }
        });
    }

    [Fact]
    public void Stack_DynamoDb_ShouldHaveDeletionPolicyDelete()
    {
        // arrange
        App app = new App();
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResource("AWS::DynamoDB::Table", new Dictionary<string, object>()
        {
            {"DeletionPolicy", "Delete"}
        });
    }

    [Fact]
    public void Stack_DynamoDb_ShouldHaveIdPartitionKey()
    {
        // arrange
        App app = new App();
        LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::DynamoDB::Table", new Dictionary<string, object>()
        {
            {
                "KeySchema", new []
                {
                    new Dictionary<string, string>()
                    {
                        {
                            "AttributeName", "Id"
                        },
                        {
                            "KeyType", "HASH"
                        }
                    }
                }
            }
        });
    }
}