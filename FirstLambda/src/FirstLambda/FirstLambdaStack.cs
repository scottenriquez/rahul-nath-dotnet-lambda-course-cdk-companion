using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using AssetOptions = Amazon.CDK.AWS.S3.Assets.AssetOptions;
using Constructs;

namespace FirstLambda
{
    public class FirstLambdaStack : Stack
    {
        internal FirstLambdaStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            IEnumerable<string> commands = new[]
            {
                "cd /asset-input",
                "export DOTNET_CLI_HOME=\"/tmp/DOTNET_CLI_HOME\"",
                "export PATH=\"$PATH:/tmp/DOTNET_CLI_HOME/.dotnet/tools\"",
                "dotnet tool install -g Amazon.Lambda.Tools",
                "dotnet lambda package -o output.zip",
                "unzip -o -d /asset-output output.zip"
            };
            Function zipLambdaFunction = new Function(this, "ZipLambdaFunction", new FunctionProps {
                Runtime = Runtime.DOTNET_6,
                Handler = "FirstLambda.ZipFunction::FirstLambda.ZipFunction.Function::FunctionHandler",
                Code = Code.FromAsset("../FirstLambda/src/FirstLambda.ZipFunction/src/FirstLambda.ZipFunction", new AssetOptions()
                {
                    Bundling = new BundlingOptions
                    {
                        Image  = Runtime.DOTNET_6.BundlingImage,
                        Command = new []
                        {
                            "bash", "-c", string.Join(" && ", commands)
                        }
                    } 
                })
            });
        }
    }
}
