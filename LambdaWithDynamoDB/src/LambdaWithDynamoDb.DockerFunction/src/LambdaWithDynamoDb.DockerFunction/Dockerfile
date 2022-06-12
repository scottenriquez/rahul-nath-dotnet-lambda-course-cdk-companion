FROM public.ecr.aws/lambda/dotnet:6-arm64 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 as build
WORKDIR /src
COPY ["LambdaWithDynamoDb.DockerFunction.csproj", "LambdaWithDynamoDb.DockerFunction/"]
RUN dotnet restore "LambdaWithDynamoDb.DockerFunction/LambdaWithDynamoDb.DockerFunction.csproj"

WORKDIR "/src/LambdaWithDynamoDb.DockerFunction"
COPY . .
RUN dotnet build "LambdaWithDynamoDb.DockerFunction.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "LambdaWithDynamoDb.DockerFunction.csproj" \
            --configuration Release \ 
            --runtime linux-arm64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["LambdaWithDynamoDb.DockerFunction::LambdaWithDynamoDb.DockerFunction.Function::FunctionHandler"]
