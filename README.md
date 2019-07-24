# dotnet-shopping
Service communicates service via Event Bus(RabbitMQ, Service Bus)

Update item in catalog api service. The message was send from the catalog.api service to the basket.api service.
The basket.api service will be saved the message on Redis.


### Requirements
+ Asp Net Core 2.2
+ Docker & Docker Compose
+ Kubernetes
+ Azure
+ Identity Server 4 (OAuth2, OpenId-Connect)
+ SQL Server
+ Redis
+ RabbitMQ on local(Service Bus on Azure)

### Structures: Copy the code from [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)
    + src
        - Catalog API
        - Basket API(Swagger using OAuth2)
        - BuildingBlocks(.NetStandard Library for reusing both catalog and basket services)
        - Identity API(Identity Server 4)
        - MapUserMVC(WebMVC using OpenId-Connect)
        - Webhooks API(Swagger using OAuth2)
        - Webhook Client(using OpenId Connect, allow to access scope of Webhooks API)
    + k8s:
        - Build service On Minikube
            All files(exclude heml-rbac.yaml, conf_map_cloud.yaml, ingress_cloud.yaml)
        - Build services on Azure Kubernetes Service(AKS)
            All files(exclude conf_map_local.yaml)
    + docker-compose.yml

### Using docker-compose
-------------------------
```
docker-compose up
```

#### Check APIs
+ Catalog API

    ```
    GET localhost/api/v1/catalog/items
    ```

    ```
    PUT localhost/api/v1/catalog/items
    ```

+ Basket API
    ```
    GET localhost/api/v1/basket/{product_id}/
    ```

### Using Kubernetes
---------------------
```
kubectl apply -f [all files].yaml
```

#### Minikube
+ Init Config Maps
    ```
    export externalDns=192.168.99.100
    kubectl create configmap urls \
        "--from-literal=BasketApiClient=http://$externalDns/basket-api" \
        "--from-literal=MvcClient=http://$externalDns/mapuser-mvc" \
        "--from-literal=WebhooksWebClient=http://$externalDns/webhooks-client" \
        "--from-literal=WebhooksApiClient=http://$externalDns/webhooks-api" \
        "--from-literal=IdentityUrlExternal=http://$externalDns/identity" \
        "--from-literal=IdentityUrl=http://$externalDns/identity"
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
    kubectl apply -f nginx-configuration.yaml
    ```

#### Check APIs
+ Catalog API

    ```
    GET localhost/catalog-api/api/v1/catalog/items
    ```

    ```
    PUT localhost/catalog-api/api/v1/catalog/items
    ```

+ Basket API
    ```
    GET localhost/api/v1/basket/{product_id}/


### Using Kubernetes on Azure
------------------------------
+ Install helm
+ Intall helm rbac
    - Error: no available release name found(https://stackoverflow.com/questions/43499971/helm-error-no-available-release-name-found)
    - Error: could not find tiller
    ```
    kubectl apply -f helm-rbac.yaml

    helm init --service-account tiller

    helm init --wait

    helm update repo
    ```

+ Create Load Balancer on Azure Helm
    ```
    helm install stable/nginx-ingress \
        --namespace default --name frontend
        --set controller.replicaCount=2 \
        --set rbac.create=false \
        --set controller.service.externalTrafficPolicy=Local \
        --set controller.nodeSelector."beta\.kubernetes\.io/os"=linux \
        --set defaultBackend.nodeSelector."beta\.kubernetes\.io/os"=linux
    ```

+ Init Config Maps
    ```
    ./init.sh
    ```

### Service Bus
+ Create Topic: "eshop_event_bus" (EntityPath: "eshop_event_bus" in Connectionstring)
    ```
    "SubscriptionClientName": "Catalog"
    "SubscriptionClientName": "Basket"
    ```

Notes:
+ [Create an ingress controller in Azure Kubernetes Server(AKS)](https://docs.microsoft.com/en-us/azure/aks/ingress-basic)
+ Pay attention to Kubernetes Ingress (https://kubernetes.github.io/ingress-nginx/examples/rewrite/)
    ```
    nginx.ingress.kubernetes.io/rewrite-target: /$2
    ```