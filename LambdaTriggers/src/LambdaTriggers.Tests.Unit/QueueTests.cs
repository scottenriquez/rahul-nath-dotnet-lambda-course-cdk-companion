using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaTriggers.Tests.Unit;

public class QueueTests
{
    [Fact]
    public void Stack_Queue_ShouldExist()
    {
        // arrange
        App app = new App();
        LambdaTriggersStack stack = new LambdaTriggersStack(app, "LambdaTriggersStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResource("AWS::SQS::Queue", new Dictionary<string, string>());
    }
}