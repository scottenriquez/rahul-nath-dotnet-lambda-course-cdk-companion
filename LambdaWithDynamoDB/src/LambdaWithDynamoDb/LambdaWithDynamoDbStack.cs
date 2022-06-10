using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.CustomResources;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace LambdaWithDynamoDb
{
    public class LambdaWithDynamoDbStack : Stack
    {
        internal LambdaWithDynamoDbStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Table dynamoDbUserTable = new Table(this, "DynamoDbUserTable", new TableProps {
                PartitionKey = new Attribute { Name = "Id", Type = AttributeType.STRING },
                // required to destroy the DynamoDB table when the stack is deleted
                RemovalPolicy = RemovalPolicy.DESTROY
            });
            // custom resource to pre-populate DynamoDB table with an item for the Lambda function to read
            AwsCustomResource loadDynamoDbDataCustomResource = new AwsCustomResource(this, "loadDynamoDbDataCustomResource", new AwsCustomResourceProps {
                OnCreate = new AwsSdkCall {
                    Service = "DynamoDB",
                    Action = "putItem",
                    Parameters = new Dictionary<string, object> {
                        {
                            "TableName", dynamoDbUserTable.TableName
                        },
                        {
                            "Item", new Dictionary<string, object>()
                            {
                                {
                                    "Id", new Dictionary<string, string>()
                                    {
                                        {
                                            "S", Guid.NewGuid().ToString()
                                        }
                                    }
                                }
                            }
                        }
                    },
                    PhysicalResourceId = PhysicalResourceId.Of($"{dynamoDbUserTable.TableName}_initialization")
                },
                Policy = AwsCustomResourcePolicy.FromSdkCalls(new SdkCallsPolicyOptions {
                    Resources = new string []
                    {
                        dynamoDbUserTable.TableArn
                    }
                })
            });
            loadDynamoDbDataCustomResource.Node.AddDependency(dynamoDbUserTable);
        }
    }
}
