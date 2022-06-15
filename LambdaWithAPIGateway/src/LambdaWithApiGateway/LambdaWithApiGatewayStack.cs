using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.CustomResources;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;
using Resource = Amazon.CDK.AWS.APIGateway.Resource;

namespace LambdaWithApiGateway
{
    public class LambdaWithApiGatewayStack : Stack
    {
        internal LambdaWithApiGatewayStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
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
                            "Item", new Dictionary<string, object>
                            {
                                {
                                    "Id", new Dictionary<string, string>
                                    {
                                        {
                                            "S", Guid.NewGuid().ToString()
                                        }
                                    }
                                },
                                {
                                    "Name", new Dictionary<string, string>
                                    {
                                        {
                                            "S", "Test item" 
                                        }
                                    }
                                }
                            }
                        }
                    },
                    PhysicalResourceId = PhysicalResourceId.Of($"{dynamoDbUserTable.TableName}_initialization")
                },
                Policy = AwsCustomResourcePolicy.FromSdkCalls(new SdkCallsPolicyOptions {
                    Resources = new[]
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
                    new ManagedPolicy(this, "ManagedPolicy", new ManagedPolicyProps
                    {
                        Document = new PolicyDocument(new PolicyDocumentProps
                        {
                            Statements = new []
                            {
                                new PolicyStatement(new PolicyStatementProps
                                {
                                    Actions = new []
                                    {
                                        "dynamodb:Query",
                                        "dynamodb:DescribeTable",
                                        "dynamodb:DeleteItem",
                                        "dynamodb:PutItem"
                                    },
                                    Effect = Effect.ALLOW,
                                    Resources = new [] { dynamoDbUserTable.TableArn }
                                }),
                                new PolicyStatement(new PolicyStatementProps
                                {
                                    Actions = new []
                                    {
                                        "logs:CreateLogGroup",
                                        "logs:CreateLogStream",
                                        "logs:PutLogEvents"
                                    },
                                    Effect = Effect.ALLOW,
                                    Resources = new [] { "*" }
                                })
                            }
                        })
                    })
                }
            });
            DockerImageCode dockerImageCode = DockerImageCode.FromImageAsset("src/LambdaWithApiGateway.DockerFunction/src/LambdaWithApiGateway.DockerFunction");
            DockerImageFunction dockerImageFunction = new DockerImageFunction(this, "ApiGatewayLambdaFunction",
                new DockerImageFunctionProps
                {
                    Architecture = Architecture.ARM_64,
                    Code = dockerImageCode,
                    Description = ".NET 6 Docker Lambda function for querying DynamoDB",
                    Environment = new Dictionary<string, string>
                    {
                        {
                            "DYNAMODB_TABLE_NAME", dynamoDbUserTable.TableName
                        }
                    },
                    Role = dockerFunctionExecutionRole,
                    Timeout = Duration.Seconds(29)
                }
            );
            LambdaRestApi api = new LambdaRestApi(this, "ApiGateway", new LambdaRestApiProps {
                Handler = dockerImageFunction,
                Proxy = false
            });
            // POST /users
            Resource usersResource = api.Root.AddResource("users");
            usersResource.AddMethod("POST");
            // GET /users/{userId}
            // DELETE /users/{userId}
            Resource userResource = usersResource.AddResource("{userId}");
            userResource.AddMethod("GET");
            userResource.AddMethod("DELETE");
        }
    }
}
