using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaTriggers.SnsDockerFunction;

public class Function
{
    /// <summary>
    /// Lambda function that processes messages events from SNS 
    /// </summary>
    /// <param name="snsEvent"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SNSEvent snsEvent, ILambdaContext context)
    {
        foreach (SNSEvent.SNSRecord snsRecord in snsEvent.Records)
        {
            await ProcessMessageAsync(snsRecord, context);
        }
    }

    private async Task ProcessMessageAsync(SNSEvent.SNSRecord snsRecord, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed record {snsRecord.Sns.Message}");
        await Task.CompletedTask;
    }
}