using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using FakeItEasy;
using Xunit;

namespace LambdaTriggers.SnsDockerFunction.Tests;

public class FunctionHandlerTests 
{
    [Fact]
    public async Task TestToUpperFunction()
    {
        // arrange
        SNSEvent snsEvent = new SNSEvent()
        {
            Records = new List<SNSEvent.SNSRecord>()
            {
                new SNSEvent.SNSRecord()
                {
                    Sns = new SNSEvent.SNSMessage()
                    {
                        Message = "Test"
                    }
                }
            }
        };
        ILambdaContext context = A.Fake<ILambdaContext>();
        Function function = new Function();
        
        // act
        await function.FunctionHandler(snsEvent, context);
        
        // assert
        A.CallTo(() => context.Logger.LogInformation("")).WithAnyArguments().MustHaveHappened();
    }
}