using dotnet_express_mapper.GraphQL;
using dotnet_express_mapper.Models;
using dotnet_express_mapper.Services;
using GraphQL.Types;

namespace dotnet_express_mapper.Queries
{
    public class ProductMutation : ObjectGraphType
    {
        public ProductMutation(ProductService productService)
        {
            Name = "Mutation";

            Field<GraphQL.ProductType>(
                "createProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
            ),
            resolve: context =>
            {
                ProductViewModel product = context.GetArgument<ProductViewModel>("product");
                return productService.Create(product);
            });

            Field<GraphQL.ProductType>(
                "updateProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }
            ),
            resolve: context =>
            {
                ProductViewModel product = context.GetArgument<ProductViewModel>("product");
                return productService.UpdateProductAsync(product);
            });
        }
    }
}
