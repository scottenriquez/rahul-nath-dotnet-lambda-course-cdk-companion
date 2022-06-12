using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LambdaWithDynamoDb.DockerFunction.Model;

namespace LambdaWithDynamoDb.DockerFunction.Repository;

public class DynamoDbUserRepository : IUserRepository
{
    public async Task<User> GetUserForIdAsync(string userGuid, string tableName)
    {
        AmazonDynamoDBClient client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        DynamoDBContext dynamoDbContext = new DynamoDBContext(client);
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = tableName,
            KeyConditionExpression = "Id = :v_Id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                {
                    ":v_Id", new AttributeValue { S =  userGuid }
                }
            }
        };
        QueryResponse queryResponse = await client.QueryAsync(queryRequest);
        Dictionary<string, AttributeValue>? items = queryResponse.Items.FirstOrDefault();
        Document document = Document.FromAttributeMap(items);
        return dynamoDbContext.FromDocument<User>(document);
    }
}