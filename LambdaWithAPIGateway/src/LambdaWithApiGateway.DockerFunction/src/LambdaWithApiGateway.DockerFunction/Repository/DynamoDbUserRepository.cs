using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LambdaWithApiGateway.DockerFunction.Model;

namespace LambdaWithApiGateway.DockerFunction.Repository;

public class DynamoDbUserRepository : IUserRepository
{
    private readonly AmazonDynamoDBClient _client;
    private readonly DynamoDBContext _dynamoDbContext;

    public DynamoDbUserRepository()
    {
        _client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        _dynamoDbContext = new DynamoDBContext(_client);
    }
    
    public async Task<User> GetUserForIdAsync(string userGuid, string tableName)
    {
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
        QueryResponse queryResponse = await _client.QueryAsync(queryRequest);
        Dictionary<string, AttributeValue>? items = queryResponse.Items.FirstOrDefault();
        Document document = Document.FromAttributeMap(items);
        return _dynamoDbContext.FromDocument<User>(document);
    }

    public async Task<User> AddUserAsync(User user, string tableName)
    {
        Table userTable = Table.LoadTable(_client, tableName);
        Document newUser = new Document
        {
            ["Id"] = user.Id,
            ["Name"] = user.Name
        };
        await userTable.PutItemAsync(newUser);
        return user;
    }

    public async Task<User> DeleteUserAsync(string userGuid, string tableName)
    {
        DeleteItemOperationConfig deleteItemOperationConfig = new DeleteItemOperationConfig
        {
            ReturnValues = ReturnValues.AllOldAttributes
        };
        Table userTable = Table.LoadTable(_client, tableName);
        Document deletedDocument = await userTable.DeleteItemAsync(userGuid, deleteItemOperationConfig);
        return new User()
        {
            Id = deletedDocument["Id"],
            Name = deletedDocument["Name"]
        };
    }
}