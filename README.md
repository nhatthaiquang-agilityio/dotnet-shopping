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

### Structures
    Copy the code from https://github.com/dotnet-architecture/eShopOnContainers
    + src
        - Catalog API
        - Basket API(Swagger using OAuth2)
        - BuildingBlocks(.NetStandard Library for reusing both catalog and basket services)
        - Identity API(Identity Server 4)
        - MapUserMVC(WebMVC using OpenId-Connect)
    + k8s:
        - Build service On Minikube
            All files(exclude heml-rbac.yaml, conf_map_cloud.yaml, ingress_cloud.yaml)
        - Build services on Azure Kubernetes Service(AKS)
            All files(exclude conf_map_local.yaml)


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