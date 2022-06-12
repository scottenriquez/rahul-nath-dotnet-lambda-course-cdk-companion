using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using LambdaWithDynamoDb.DockerFunction.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaWithDynamoDb.DockerFunction;

public class Function
{
    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<User> FunctionHandler(Input input, ILambdaContext context)
    {
        AmazonDynamoDBClient client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        DynamoDBContext dynamoDbContext = new DynamoDBContext(client);
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME"),
            KeyConditionExpression = "Id = :v_Id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                {
                    ":v_Id", new AttributeValue { S =  input.Id }
                }
            }
        };
        QueryResponse queryResponse = await client.QueryAsync(queryRequest);
        Dictionary<string, AttributeValue>? items = queryResponse.Items.FirstOrDefault();
        Document document = Document.FromAttributeMap(items);
        return dynamoDbContext.FromDocument<User>(document);
    }
}