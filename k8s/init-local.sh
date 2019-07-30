#!/usr/bin/env bash
externalDns=$(minikube ip)
echo "Using $externalDns as the external IP of the Minikube"
echo $externalDns

kubectl delete configmap urls || true

# urls configmap
kubectl create configmap urls \
    "--from-literal=BasketApiClient=http://$externalDns/basket-api" \
    "--from-literal=OrderingApiClient=http://$externalDns/orders-api" \
    "--from-literal=MapUserMvcClient=http://$externalDns/mapuser-mvc" \
    "--from-literal=MvcClient=http://$externalDns/web-mvc" \
    "--from-literal=WebhooksWebClient=http://$externalDns/webhooks-client" \
    "--from-literal=WebhooksApiClient=http://$externalDns/webhooks-api" \
    "--from-literal=WebShoppingAggClient=http://$externalDns/webshoppingagg" \
    "--from-literal=IdentityUrlExternal=http://$externalDns/identity" \
    "--from-literal=IdentityUrl=http://$externalDns/identity" \
    "--from-literal=webshoppingapigw_e=http://$externalDns/webshoppingapigw"