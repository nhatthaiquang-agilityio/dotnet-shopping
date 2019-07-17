#!/usr/bin/env bash
echo "#################### Waiting for Azure to provision external IP ####################"

ip_regex='([0-9]{1,3}\.){3}[0-9]{1,3}'
while true; do
    printf "."
    frontendUrl=$(kubectl get svc frontend-nginx-ingress-controller -o=jsonpath="{.status.loadBalancer.ingress[0].ip}")
    if [[ $frontendUrl =~ $ip_regex ]]; then
        break
    fi
    sleep 5s
done

printf "\n"
externalDns=$frontendUrl
echo "Using $externalDns as the external DNS/IP of the K8s cluster"

echo "#################### Creating application configuration ####################"

# urls configmap
kubectl create configmap urls \
    "--from-literal=BasketApiClient=http://$externalDns/basket-api" \
    "--from-literal=IdentityUrlExternal=http://$externalDns/identity" \
    "--from-literal=IdentityUrl=http://$externalDns/identity"
kubectl label configmap urls app=eshop