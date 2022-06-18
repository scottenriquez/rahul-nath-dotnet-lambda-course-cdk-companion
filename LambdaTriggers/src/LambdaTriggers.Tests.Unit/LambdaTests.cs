using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.Assertions;
using Xunit;

namespace LambdaTriggers.Tests.Unit;

public class LambdaTests
{
    [Fact]
    public void Stack_SnsLambda_ShouldHaveTopicSubscription()
    {
        // arrange
        App app = new App();
        LambdaTriggersStack stack = new LambdaTriggersStack(app, "LambdaTriggersStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::SNS::Subscription", new Dictionary<string, string>()
        {
            {
                "Protocol", "lambda"
            }
        });
    }
    
    [Fact]
    public void Stack_SqsLambda_ShouldHaveQueueEventMapping()
    {
        // arrange
        App app = new App();
        LambdaTriggersStack stack = new LambdaTriggersStack(app, "LambdaTriggersStack");

        // act
        Template template = Template.FromStack(stack);

        // assert
        template.HasResourceProperties("AWS::Lambda::EventSourceMapping", new Dictionary<string, object>()
        {
            {
                "EventSourceArn", new Dictionary<string, object[]>()
                {
                    {
                        "Fn::GetAtt", new object []
                        {
                            Match.StringLikeRegexp("Queue"),
                            "Arn"
                        }
                    }
                } 
            }
        });
    }
}