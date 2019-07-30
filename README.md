# dotnet-shopping
Service communicates service via Event Bus(RabbitMQ, Service Bus)

Update item in catalog api service. The message was send from the catalog.api service to the basket.api service.
The basket.api service will be saved the message on Redis.


## Requirements
+ Asp Net Core 2.2
+ Docker & Docker Compose
+ Kubernetes
+ Azure
    - Store Logging
    - Application Insight
    - Service Bus
    - Redis Server
+ Identity Server 4 (OAuth2, OpenId-Connect)
+ API Gateway(Using Ocelot)
+ SQL Server
+ Redis (Store Data Protection Key)
+ RabbitMQ on local(Service Bus on Azure)


### Structures: Copy the code from [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)
-----------------------------------------------------------------------------------------------------------------
    + src
        - ApiGateways (Building API gateway Using Ocelot)
            - ApiGw-Base (Ocelot API Gateway)
            - Web.Shopping aggregator
            - apigw-config (Ocelot Configuration)
        - Catalog API
        - Basket API(Swagger using OAuth2)
        - BuildingBlocks(.NetStandard Library for reusing both catalog and basket services)
        - Identity API(Identity Server 4)
        - MapUserMVC(WebMVC using OpenId-Connect)
        - Web MVC
        - Ordering API
        - Webhooks API(Swagger using OAuth2)
        - Webhook Client(using OpenId Connect, allow to access scope of Webhooks API)
    + k8s:
        - configmaps
        - ingress-controller
        - ocelot (API gateway)
        - service files
    + docker-compose.yml

#### Notes
+ Build service On Minikube:
    - All files(exclude heml-rbac.yaml, conf-map-cloud.yaml, ingress-cloud.yaml)
+ Build services on Azure Kubernetes Service(AKS):
    - All files(exclude conf-map-local.yaml)


### Using docker-compose
-------------------------
```
docker-compose up
```

#### Issues on Docker Compose
-------------------------------
+ Got exception IDX20804: Unable to retrieve document from: <path>/.well-known/openid-configuration'. IDX20803: Unable to obtain configuration from: '[PII is hidden]'.
    - https://www.gitmemory.com/issue/IdentityServer/IdentityServer4/2337/467712399
    - Fixed: https://github.com/IdentityServer/IdentityServer4/issues/2450 (Using Local Mac in docker)

#### Check APIs
+ Catalog API

    ```
    GET localhost:6003/api/v1/catalog/items
    ```

    ```
    PUT localhost:6003/api/v1/catalog/items
    ```

+ Basket API
    ```
    GET localhost:6002/api/v1/basket/{product_id}/
    ```

+ Identity Server
    ```
    http://localhost:6001/
    ```

+ API Gateway
    ```
    http://localhost:6009/api/v1/c/catalog/items
    ```

+ Webhook Client
    ```
    http://localhost:6005/
    ```

+ Webhooks API
    ```
    http://localhost:6006/
    ```

### Using Kubernetes
---------------------
```
kubectl apply -f [service files].yaml
```

#### Minikube
+ Init Config Maps URLs
    ```
    export externalDns=192.168.99.100
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
        "--from-literal=webshoppingapigw_e=http://$($externalDns)/webshoppingapigw"
    ```

+ Init Ocelot(k8s/configmaps/config-map-ocelot.sh)
    ```
    kubectl create configmap ocelot --from-file=ws=ocelot/configuration.json
    ```

+ Init Interal Urls
    ```
    kubectl apply -f configmaps/internalurls.yaml
    ```

+ Init External Config
    ```
    kubectl apply -f configmaps/conf-map-local.yaml
    ```

#### Issues On Minikube
-------------------------
+ Using Antiforgery in ASP.NET Core and got error - the antiforgery token could not be decrypted
    https://stackoverflow.com/questions/42103004/using-antiforgery-in-asp-net-core-and-got-error-the-antiforgery-token-could-no
    - Fixed: Using Data Protection(store key in Redis)

+ signin-oidc callback fails with 502 Bad Gateway
    https://github.com/IdentityServer/IdentityServer4/issues/2101
    https://github.com/aspnet/Security/issues/1049

    ```
    kubectl get configmap -n kube-system
    ```
    - Nginx Controller: using nginx-load-balancer-conf config map
    - Get logging: 43#43: *5928 upstream sent too big header while reading response header from upstream, client: 192.168.99.1, server: _, request: "POST /mapuser-mvc/signin-oidc HTTP/1.1", upstream: "http://172.17.0.9:80/mapuser-mvc/signin-oidc", host: "192.168.99.100"
    - Fixed: add `proxy-buffer-size: "128k"` (https://medium.com/@mshanak/solve-nginx-error-signin-oidc-502-bad-gateway-dotnet-core-and-identity-serve-bc27920b42d5)
    ```
    kubectl apply -f configmaps/nginx-configuration.yaml
    ```

#### Check APIs
+ Catalog API

    ```
    GET  http://192.168.99.100/catalog-api/api/v1/catalog/items
    ```

    ```
    PUT  http://192.168.99.100/catalog-api/api/v1/catalog/items
    ```

+ Basket API
    ```
    GET  http://192.168.99.100/api/v1/basket/{product_id}/
    ```

+ Identiy Server
    ```
    Get http://192.168.99.100/identity
    ```

+ Map User MVC
    ```
    http://192.168.99.100/mapuser-mvc
    ```

+ Webhook Client
    ```
    http://192.168.99.100/webhooks-client
    ```

+ Webhooks APIs
    ```
    http://192.168.99.100/webhooks-api
    ```

+ API Gateway
    ```
    GET  http://192.168.99.100/webshoppingapigw/catalog-api/api/v1/catalog/items
    ```

+ Web shopping Aggregator
    ```
    http://192.168.99.100/webshoppingagg/index.html
    ```


### Using Kubernetes on Azure
------------------------------
#### Install helm & helm rbac
    - Error: no available release name found
        (https://stackoverflow.com/questions/43499971/helm-error-no-available-release-name-found)
    - Error: could not find tiller
    ```
    kubectl apply -f ingress-controller/helm-rbac.yaml

    helm init --service-account tiller

    helm init --wait

    helm update repo
    ```

#### Create Load Balancer on Azure Helm
    Note: Wait a few minutes for creating public IP
    ```
    helm install stable/nginx-ingress \
        --namespace default --name frontend
        --set controller.replicaCount=2 \
        --set rbac.create=false \
        --set controller.service.externalTrafficPolicy=Local \
        --set controller.nodeSelector."beta\.kubernetes\.io/os"=linux \
        --set defaultBackend.nodeSelector."beta\.kubernetes\.io/os"=linux
    ```

#### Init Config Maps
    ```
    ./init.sh
    ```

#### Init Ocelot(k8s/configmaps/config-map-ocelot.sh)
    ```
    kubectl create configmap ocelot --from-file=ws=ocelot/configuration.json
    ```

#### Init Interal Urls
    ```
    kubectl apply -f configmaps/internal.yaml
    ```

#### Init externalcfg
    ```
    kubectl apply -f configmaps/conf-map-cloud.yaml
    ```

### Service Bus
----------------
+ Create Topic: "eshop_event_bus" (EntityPath: "eshop_event_bus" in Connectionstring)
    ```
    "SubscriptionClientName": "Catalog"
    "SubscriptionClientName": "Basket"
    ```


### Reference
--------------
+ [Building API gateway Using Ocelot](http://www.pogsdotnet.com/2018/08/building-simple-api-gateways-with.html)
+ [Designing and implementing API Gateways with Ocelot in .NET Core containers and microservices architectures](https://devblogs.microsoft.com/cesardelatorre/designing-and-implementing-api-gateways-with-ocelot-in-a-microservices-and-container-based-architecture/)


## Notes
+ [Create an ingress controller in Azure Kubernetes Server(AKS)](https://docs.microsoft.com/en-us/azure/aks/ingress-basic)
+ Pay attention to Kubernetes Ingress (https://kubernetes.github.io/ingress-nginx/examples/rewrite/)
    ```
    nginx.ingress.kubernetes.io/rewrite-target: /$2
    ```