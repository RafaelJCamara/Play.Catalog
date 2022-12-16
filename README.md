# Play.Catalog
Catalog microservice.

## Create and publish Catalog.Contracts NuGet package
```powershell
$version="1.0.3"
$owner="RafaelJCamara"
$gh_pat="[PERSONAL ACCESS TOKEN HERE]"

dotnet pack src\Play.Catalog.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Catalog -o ..\packages

dotnet nuget push ..\packages\Play.Catalog.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build the docker image
```powershell
$version="1.0.3"
$env:GH_OWNER="RafaelJCamara"
$env:GH_PAT="[PERSONAL ACCESS TOKEN HERE]"

docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.catalog:$version .
```


## Run the docker image
```powershell
$version="1.0.3"

docker run -it --rm -p 5000:5000 --name catalog -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq --network playinfra_default play.catalog:$version
```