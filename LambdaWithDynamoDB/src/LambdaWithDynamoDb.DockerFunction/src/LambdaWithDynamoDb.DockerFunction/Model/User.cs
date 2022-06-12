using Amazon.DynamoDBv2.DataModel;

namespace LambdaWithDynamoDb.DockerFunction.Model;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
}