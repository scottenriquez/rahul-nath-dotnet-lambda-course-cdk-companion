using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
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
            Role dockerFunctionExecutionRole = new Role(this, "DockerFunctionExecutionRole", new RoleProps {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
                ManagedPolicies = new IManagedPolicy[]
                {
                    new ManagedPolicy(this, "ManagedPolicy", new ManagedPolicyProps()
                    {
                        Document = new PolicyDocument(new PolicyDocumentProps()
                        {
                            Statements = new []
                            {
                                new PolicyStatement(new PolicyStatementProps()
                                {
                                    Actions = new [] { "dynamodb:Query" },
                                    Resources = new [] { dynamoDbUserTable.TableArn }
                                })
                            }
                        })
                    })
                }
            });
            DockerImageCode dockerImageCode = DockerImageCode.FromImageAsset("src/LambdaWithDynamoDb.DockerFunction/src/LambdaWithDynamoDb.DockerFunction");
            DockerImageFunction dockerImageFunction = new DockerImageFunction(this, "LambdaFunction",
                new DockerImageFunctionProps()
                {
                    Architecture = Architecture.ARM_64,
                    Code = dockerImageCode,
                    Description = ".NET 6 Docker Lambda function for querying DynamoDB",
                    Environment = new Dictionary<string, string>()
                    {
                        {
                            "DYNAMODB_TABLE_NAME", dynamoDbUserTable.TableName
                        }
                    },
                    Role = dockerFunctionExecutionRole,
                    Timeout = Duration.Minutes(5) 
                }
            );
        }
    }
}
