## The Course and Companion
[Rahul Nath](https://www.rahulpnath.com/) recently released a course called AWS Lambda for the .NET Developer on [Udemy](https://www.udemy.com/course/aws-lambda-dotnet/) and [Gumroad](https://rahulpnath.gumroad.com/l/aws-lambda-dot-net). I had a ton of fun going through the exercises and highly recommend purchasing a copy. While working through the material, I implemented the solutions with infrastructure as code using AWS CDK in C# and .NET 6. I also containerized most of the Lambda functions and wrote unit tests for both the functions and infrastructure.

## Technology Decisions and Benefits
While infrastructure as code (IaC) has existed within the AWS ecosystem for over a decade, adoption has exploded in recent years due to the ability to manage large amounts of infrastructure at scale and standardize design across an organization. There are many options including CloudFormation (CFN), CDK, and Terraform for IaC and Serverless Application Model (SAM) and Serverless Framework for development. [This article from A Cloud Guru](https://acloudguru.com/blog/engineering/cloudformation-terraform-or-cdk-guide-to-iac-on-aws) quickly sums up the pros and cons of each IaC option. I choose this particular stack for some key reasons:
- Docker ensures that the Lambda functions run consistently across local development, builds, and production environments and simplifies dependency management
- CDK allows the infrastructure to be described as C# instead of YAML, JSON, or HCL 
- CDK provides the ability to inject more robust logic than intrinsic functions in CloudFormation and more modularity as well while still being an AWS-supported offering
- CDK supports unit testing

Elaborating on the final point, here is an example unit test for ensuring that a DynamoDB table is destroyed when the stack is. The default behavior is for the table to be retained, leading to clutter and cost since this is a non-production project. This is an example how of IaC can be meaningfully tested:
```csharp
[Fact]
public void Stack_DynamoDb_ShouldHaveDeletionPolicyDelete()
{
    // arrange
    App app = new App();
    LambdaWithApiGatewayStack stack = new LambdaWithApiGatewayStack(app, "LambdaWithApiGatewayStack");

    // act
    Template template = Template.FromStack(stack);

    // assert
    template.HasResource("AWS::DynamoDB::Table", new Dictionary<string, object>()
    {
        {"DeletionPolicy", "Delete"}
    });
}
```

## Dependencies
To build and run this codebase, the following dependencies must be installed:
- .NET 6
- Node.js
- Docker
- AWS CDK
- Credentials configured in `~/.aws/credentials`(easily done with the AWS CLI)

## My Development Environment and CPU Architecture Considerations
I developed all the code on my M1 MacBook Pro using JetBrains Rider. Because of my machine's ARM processor, it's key to note that all of my Dockerfiles use ARM images (e.g., `public.ecr.aws/lambda/dotnet:6-arm64`) and are deployed to [Graviton2 Lambda environments](https://aws.amazon.com/blogs/aws/aws-lambda-functions-powered-by-aws-graviton2-processor-run-your-functions-on-arm-and-get-up-to-34-better-price-performance/). I suspect that most folks reading this are using x86 Windows machines, so here is a modified Dockerfile illustrating the requisite changes:
```dockerfile
# ARM
# FROM public.ecr.aws/lambda/dotnet:6-arm64 AS base
# x86
FROM public.ecr.aws/lambda/dotnet:6 AS base

# ARM
# FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 as build
# x86
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim as build
WORKDIR /src
COPY ["LambdaWithApiGateway.DockerFunction.csproj", "LambdaWithApiGateway.DockerFunction/"]
RUN dotnet restore "LambdaWithApiGateway.DockerFunction/LambdaWithApiGateway.DockerFunction.csproj"

WORKDIR "/src/LambdaWithApiGateway.DockerFunction"
COPY . .
RUN dotnet build "LambdaWithApiGateway.DockerFunction.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "LambdaWithApiGateway.DockerFunction.csproj" \
            --configuration Release \ 
            # ARM
            # --runtime linux-arm64
            # x86
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["LambdaWithApiGateway.DockerFunction::LambdaWithApiGateway.DockerFunction.Function::FunctionHandler"]
```

The CDK code for the Lambda function also requires a slight change:
```csharp
DockerImageFunction sqsDockerImageFunction = new DockerImageFunction(this, "LambdaFunction",
    new DockerImageFunctionProps()
    {
        // ARM
        // Architecture = Architecture.ARM_64,
        // x86
        Architecture = Architecture.X86_64,
        Code = sqsDockerImageCode,
        Description = ".NET 6 Docker Lambda function for polling SQS",
        Role = sqsDockerFunctionExecutionRole,
        Timeout = Duration.Seconds(30) 
    }
);
```

## Using Cloud9
AWS offers a browser-based IDE called Cloud9 that has nearly all required dependencies installed. The IDE can be provisioned from the AWS Console or via infrastructure as code. Unfortunately, Cloud9 does not support Graviton-based instances yet. Below is a CloudFormation template for provisioning an environment with the source code pre-loaded:
```yaml
Resources:
  rCloud9Environment:
    Type: AWS::Cloud9::EnvironmentEC2
    Properties:
      AutomaticStopTimeMinutes: 30
      ConnectionType: CONNECT_SSM
      Description: Web-based cloud development environment
      InstanceType: m5.large	
      Name: Cloud9Environment
      Repositories: 
        - PathComponent: /repos/rahul-nath-dotnet-lambda-course-cdk-companion
          RepositoryUrl: https://github.com/scottenriquez/rahul-nath-dotnet-lambda-course-cdk-companion.git
```

Note that the instance must be deployed to a public subnet. The Cloud9 AMI does not have .NET 6 pre-installed. To do so, run the following commands:
```shell
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install dotnet-sdk-6.0
```

## Code Structure
Each section of the course has a separate solution in the repository:
- `FirstLambda` is a simple ZIP Lambda function that returns the uppercase version of a string 
- `LambdaWithDynamoDb` is a simple Lambda function that queries a DynamoDB table
- `LambdaWithApiGateway` is a full CRUD app using DynamoDB for storage 
- `LambdaTriggers` are event-driven Lambda functions triggered by SNS and SQS

Each solution is structured in the same way. I generated the CDK app using the CLI and used the Lambda templates to create my functions like so:
```shell
# create the CDK application
# the name is derived from the directory
# this snippet assumes the directory is called Lambda
cdk init app --language csharp
# install the latest version of the .NET Lambda templates
dotnet new -i Amazon.Lambda.Templates
cd src/
# create the function
dotnet new lambda.image.EmptyFunction --name Lambda.DockerFunction
# add the projects to the solution file
dotnet sln add Lambda.DockerFunction/src/Lambda.DockerFunction/Lambda.DockerFunction.csproj
dotnet sln add Lambda.DockerFunction/test/Lambda.DockerFunction.Tests/Lambda.DockerFunction.Tests.csproj
# build the solution and run the sample unit test to verify that everything is wired up correctly
dotnet test Lambda.sln
```

Each Lambda function has projects for the handler code and unit tests. All CDK code for infrastructure resides in the corresponding `*Stack.cs` file. Here is some example IaC for a Lambda function triggered by SQS:
```csharp
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
```

## Resource Deployment
To deploy the infrastructure, navigate to the corresponding section folder and use the CDK CLI like so:
```shell
cd LambdaTriggers
cdk deploy
```

## Resource Cleanup
To destroy resources, run this command in the same directory:
```shell
cdk destroy
```
