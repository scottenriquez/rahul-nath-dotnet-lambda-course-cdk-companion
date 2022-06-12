using Amazon.Lambda.Core;
using Xunit;
using Amazon.Lambda.TestUtilities;
using LambdaWithDynamoDb.DockerFunction.Model;

namespace LambdaWithDynamoDb.DockerFunction.Tests;

public class FunctionIntegrationTests
{
    [Fact (Skip = "For local development only")]
    public async void TestToUpperFunction()
    {
        Function function = new Function();
        ILambdaContext context = new TestLambdaContext();
        User user = await function.FunctionHandler(new Input() { Id = "e6f0ca6d-ba1c-4eec-9e2f-672e8f92447f" }, context);
    }
}