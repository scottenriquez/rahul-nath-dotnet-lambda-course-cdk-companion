FROM public.ecr.aws/lambda/dotnet:6-arm64 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 as build
WORKDIR /src
COPY ["LambdaTriggers.SqsDockerFunction.csproj", "LambdaTriggers.SqsDockerFunction/"]
RUN dotnet restore "LambdaTriggers.SqsDockerFunction/LambdaTriggers.SqsDockerFunction.csproj"

WORKDIR "/src/LambdaTriggers.SqsDockerFunction"
COPY . .
RUN dotnet build "LambdaTriggers.SqsDockerFunction.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "LambdaTriggers.SqsDockerFunction.csproj" \
            --configuration Release \ 
            --runtime linux-arm64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["LambdaTriggers.SqsDockerFunction::LambdaTriggers.SqsDockerFunction.Function::FunctionHandler"]