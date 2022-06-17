using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaWithDynamoDb.Tests.Unit;

public class StackTests 
{
    [Fact]
    public void Stack_DynamoDb_ShouldHaveFiveRcuAndWcu()
    {
        // arrange
        App app = new App();
        LambdaWithDynamoDbStack stack = new LambdaWithDynamoDbStack(app, "LambdaWithDynamoDbStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::DynamoDB::Table", new Dictionary<string, object>()
        {
            { "ProvisionedThroughput", new Dictionary<string, int>()
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
        LambdaWithDynamoDbStack stack = new LambdaWithDynamoDbStack(app, "LambdaWithDynamoDbStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResource("AWS::DynamoDB::Table", new Dictionary<string, object>()
        {
            { "DeletionPolicy", "Delete" }
        });
    }
}