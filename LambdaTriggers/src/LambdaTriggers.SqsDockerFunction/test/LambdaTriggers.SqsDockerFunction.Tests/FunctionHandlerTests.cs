using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using FakeItEasy;

namespace LambdaTriggers.SqsDockerFunction.Tests;

public class FunctionHandlerTest
{
    [Fact]
    public async Task Should_FunctionHandler_ForStandardInput()
    {
        // arrange
        SQSEvent sqsEvent = new SQSEvent()
        {
            Records = new List<SQSEvent.SQSMessage>()
            {
                new SQSEvent.SQSMessage()
                {
                    Body = "Test"
                }
            }
        };
        ILambdaContext context = A.Fake<ILambdaContext>();
        Function function = new Function();
        
        // act
        await function.FunctionHandler(sqsEvent, context);
        
        // assert
        A.CallTo(() => context.Logger.LogInformation("")).WithAnyArguments().MustHaveHappened();
    }
}