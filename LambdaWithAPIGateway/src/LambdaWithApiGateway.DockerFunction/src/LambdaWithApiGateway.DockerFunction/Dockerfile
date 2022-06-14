FROM public.ecr.aws/lambda/dotnet:6-arm64 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 as build
WORKDIR /src
COPY ["LambdaWithApiGateway.DockerFunction.csproj", "LambdaWithApiGateway.DockerFunction/"]
RUN dotnet restore "LambdaWithApiGateway.DockerFunction/LambdaWithApiGateway.DockerFunction.csproj"

WORKDIR "/src/LambdaWithApiGateway.DockerFunction"
COPY . .
RUN dotnet build "LambdaWithApiGateway.DockerFunction.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "LambdaWithApiGateway.DockerFunction.csproj" \
            --configuration Release \ 
            --runtime linux-arm64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["LambdaWithApiGateway.DockerFunction::LambdaWithApiGateway.DockerFunction.Function::FunctionHandler"]