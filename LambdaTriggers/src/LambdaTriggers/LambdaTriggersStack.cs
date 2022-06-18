using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
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
                    new ManagedPolicy(this, "ManagedPolicy", new ManagedPolicyProps()
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
            DockerImageFunction sqsDockerImageFunction = new DockerImageFunction(this, "LambdaFunction",
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
        }
    }
}
