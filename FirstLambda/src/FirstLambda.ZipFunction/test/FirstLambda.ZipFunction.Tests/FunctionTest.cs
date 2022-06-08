using Xunit;
using Amazon.Lambda.TestUtilities;

namespace FirstLambda.ZipFunction.Tests;

public class FunctionTest
{
    [Fact]
    public void TestToUpperFunction()
    {
        // arrange
        Function function = new Function();
        FunctionInput functionInput = new FunctionInput()
        {
            word = "hello world"
        };
        TestLambdaContext context = new TestLambdaContext();
        
        // act
        string upperCase = function.FunctionHandler(functionInput, context);

        // assert
        Assert.Equal("HELLO WORLD", upperCase);
    }
}
