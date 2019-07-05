# dotnet-shopping
Service communicates service via Event Bus(RabbitMQ, Service Bus)

Update item in catalog api service. The message was send from the catalog.api service to the basket.api service.
The basket.api service will be saved the message on Redis.


### Requirements
+ Asp Net Core 2.2
+ Docker & Docker Compose
+ Kubernetes
+ Azure
+ SQL Server
+ Redis
+ RabbitMQ on local(Service Bus on Azure)

### Structures
    Copy the code from https://github.com/dotnet-architecture/eShopOnContainers
    + src
        - Catalog API
        - Basket API
        - BuildingBlocks(.NetStandard Library for reusing both catalog and basket services)
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

Notes:
+ Pay attention to Kubernetes Ingress (https://kubernetes.github.io/ingress-nginx/examples/rewrite/)
    - nginx.ingress.kubernetes.io/rewrite-target: /$2