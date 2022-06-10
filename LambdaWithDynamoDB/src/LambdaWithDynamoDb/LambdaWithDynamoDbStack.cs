using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace LambdaWithDynamoDb
{
    public class LambdaWithDynamoDbStack : Stack
    {
        internal LambdaWithDynamoDbStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Table dynamoDbUserTable = new Table(this, "DynamoDbUserTable", new TableProps {
                PartitionKey = new Attribute { Name = "Id", Type = AttributeType.STRING }
            });
        }
    }
}
