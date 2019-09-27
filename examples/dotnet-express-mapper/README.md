# dotnet-express-mapper
+ Apply Express Mapper instead of AutoMapper
+ One to Many Relationships in Entity Framework Core
+ Many to Many Relationships in Entity Framework Core
+ Seed Data
+ GraphQL


### Prerequisite
+ Asp Net Core 2.2
+ Entity Framework
+ Express Mapper
+ Sql Server
+ Docker & Docker Compose


### Usage

+ GraphQL
    + Check GraphQL schema
    ```
    {
        __schema {
            types {
                name
            }
        }
    }
    ```

    + Query Book by Id
    ```
    http://localhost:5000/graphql

    query BookQuery($id: Int) {
        book(id: $id) {
            id
            bookName
            price
            author {
                id
                firstName
            }

            bookCategories {
                bookId
                categoryId

                category {
                    categoryId
                    categoryName
                }
            }
        }
    }


    Query variable
    {
        "id": 1
    }
    ```

    Output
    ```
    {
        "data": {
            "book": {
            "id": 1,
            "bookName": "Quantum Networking",
            "price": 220,
            "author": {
                "id": 1,
                "firstName": "Nick"
            },
            "bookCategories": [
                {
                "bookId": 1,
                "categoryId": 1,
                "category": {
                    "categoryId": 1,
                    "categoryName": "Network"
                }
                }
            ]
            }
        }
    }
    ```

    + Query All Books
    ```
    query BookQuery {
        books {
            id
            bookName
            price

            author {
                id
                firstName
            }

            bookCategories {
                bookId
                categoryId

                category {
                    categoryId
                    categoryName
                }
            }
        }
    }
    ```

    Output
    ```
    {
        "data": {
            "books": [
            {
                "id": 1,
                "bookName": "Quantum Networking",
                "price": 220,
                "author": {
                "id": 1,
                "firstName": "Nick"
                },
                "bookCategories": [
                {
                    "bookId": 1,
                    "categoryId": 1,
                    "category": {
                    "categoryId": 1,
                    "categoryName": "Network"
                    }
                }
                ]
            },
            {
                "id": 2,
                "bookName": "Advance C#",
                "price": 110,
                "author": {
                "id": 2,
                "firstName": "David"
                },
                "bookCategories": [
                {
                    "bookId": 2,
                    "categoryId": 2,
                    "category": {
                    "categoryId": 2,
                    "categoryName": "Programming"
                    }
                }
                ]
            }
            ]
        }
    }
    ```
### Reference
+ [Express Mapper](http://expressmapper.org/)
+ [Configuring Many To Many Relationships in Entity Framework Core](https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration)
+ [Configuring One To Many Relationships in Entity Framework Core](https://www.learnentityframeworkcore.com/configuration/one-to-many-relationship-configuration)
+ [Update many to many relationships in Entity Framework Core](https://www.thereformedprogrammer.net/updating-many-to-many-relationships-in-entity-framework-core/)
+ [Building a GraphQL API with ASP.NET Core 2 and Entity Framework Core](https://fullstackmark.com/post/17/building-a-graphql-api-with-aspnet-core-2-and-entity-framework-core)
+ [Build a GraphQL API with ASP.NET Core](https://developer.okta.com/blog/2019/04/16/graphql-api-with-aspnetcore)