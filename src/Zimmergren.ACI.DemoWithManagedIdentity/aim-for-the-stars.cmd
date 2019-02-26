dotnet build && dotnet publish
docker build . -t aci-demo-app-with-managed-identity
docker tag aci-demo-app-with-managed-identity acrdemomagic.azurecr.io/aci-demo-app-with-managed-identity:latest
docker push acrdemomagic.azurecr.io/aci-demo-app-with-managed-identity:latest
REM az container delete -g demos -n aci-demo-app-with-managed-identity
REM az container create -g demos -f ..\..\yaml\aci-demo-with-managed-identity.yaml