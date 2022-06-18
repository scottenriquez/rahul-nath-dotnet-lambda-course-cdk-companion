using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaTriggers.Tests.Unit;

public class TopicTests 
{
    [Fact]
    public void Stack_Topic_ShouldExist()
    {
        // arrange
        App app = new App();
        LambdaTriggersStack stack = new LambdaTriggersStack(app, "LambdaTriggersStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResource("AWS::SNS::Topic", new Dictionary<string, string>());
    }
}