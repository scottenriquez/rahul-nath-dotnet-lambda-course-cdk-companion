FROM public.ecr.aws/lambda/dotnet:6-arm64 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 as build
WORKDIR /src
COPY ["LambdaTriggers.SnsDockerFunction.csproj", "LambdaTriggers.SnsDockerFunction/"]
RUN dotnet restore "LambdaTriggers.SnsDockerFunction/LambdaTriggers.SnsDockerFunction.csproj"

WORKDIR "/src/LambdaTriggers.SnsDockerFunction"
COPY . .
RUN dotnet build "LambdaTriggers.SnsDockerFunction.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "LambdaTriggers.SnsDockerFunction.csproj" \
            --configuration Release \ 
            --runtime linux-arm64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["LambdaTriggers.SnsDockerFunction::LambdaTriggers.SnsDockerFunction.Function::FunctionHandler"]