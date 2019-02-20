dotnet build && dotnet publish
docker build . -t aci-demo-app-with-secrets-mount
docker tag aci-demo-app-with-secrets-mount acrdemomagic.azurecr.io/aci-demo-app-with-secrets-mount:latest
docker push acrdemomagic.azurecr.io/aci-demo-app-with-secrets-mount:latest
REM az container delete -g demos -n aci-demo-app-with-secrets-mount
REM az container create -g demos -f ..\..\yaml\aci-demo-with-secret-volume-mount.yaml