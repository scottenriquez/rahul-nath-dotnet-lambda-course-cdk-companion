using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace LambdaTriggers
{
    public class LambdaTriggersStack : Stack
    {
        public LambdaTriggersStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Queue queue = new Queue(this, "Queue");
            Role sqsDockerFunctionExecutionRole = new Role(this, "SqsDockerFunctionExecutionRole", new RoleProps {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
                ManagedPolicies = new IManagedPolicy[]
                {
                    new ManagedPolicy(this, "SqsManagedPolicy", new ManagedPolicyProps()
                    {
                        Document = new PolicyDocument(new PolicyDocumentProps()
                        {
                            Statements = new []
                            {
                                new PolicyStatement(new PolicyStatementProps()
                                {
                                    Actions = new [] { "sqs:*" },
                                    Resources = new [] { queue.QueueArn }
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
            DockerImageCode sqsDockerImageCode = DockerImageCode.FromImageAsset("src/LambdaTriggers.SqsDockerFunction/src/LambdaTriggers.SqsDockerFunction");
            DockerImageFunction sqsDockerImageFunction = new DockerImageFunction(this, "SqsLambdaFunction",
                new DockerImageFunctionProps()
                {
                    Architecture = Architecture.ARM_64,
                    Code = sqsDockerImageCode,
                    Description = ".NET 6 Docker Lambda function for polling SQS",
                    Role = sqsDockerFunctionExecutionRole,
                    Timeout = Duration.Seconds(30) 
                }
            );
            SqsEventSource sqsEventSource = new SqsEventSource(queue);
            sqsDockerImageFunction.AddEventSource(sqsEventSource);
            Topic topic = new Topic(this, "Topic", new TopicProps {
                DisplayName = "Topic"
            });
            Role snsDockerFunctionExecutionRole = new Role(this, "SnsDockerFunctionExecutionRole", new RoleProps {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
                ManagedPolicies = new IManagedPolicy[]
                {
                    new ManagedPolicy(this, "SnsManagedPolicy", new ManagedPolicyProps()
                    {
                        Document = new PolicyDocument(new PolicyDocumentProps()
                        {
                            Statements = new []
                            {
                                new PolicyStatement(new PolicyStatementProps()
                                {
                                    Actions = new [] { "sns:*" },
                                    Resources = new [] { topic.TopicArn }
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
            DockerImageCode snsDockerImageCode = DockerImageCode.FromImageAsset("src/LambdaTriggers.SnsDockerFunction/src/LambdaTriggers.SnsDockerFunction");
            DockerImageFunction snsDockerImageFunction = new DockerImageFunction(this, "SnsLambdaFunction",
                new DockerImageFunctionProps()
                {
                    Architecture = Architecture.ARM_64,
                    Code = snsDockerImageCode,
                    Description = ".NET 6 Docker Lambda function for polling SNS",
                    Role = snsDockerFunctionExecutionRole,
                    Timeout = Duration.Seconds(30) 
                }
            );
            SnsEventSource snsEventSource = new SnsEventSource(topic);
            snsDockerImageFunction.AddEventSource(snsEventSource);
        }
    }
}
