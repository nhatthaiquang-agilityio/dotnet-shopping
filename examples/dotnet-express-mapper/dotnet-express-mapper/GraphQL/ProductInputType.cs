using dotnet_express_mapper.Models;
using GraphQL.Types;

namespace dotnet_express_mapper.GraphQL
{
    public class ProductInputType : InputObjectGraphType<ProductViewModel>
    {
        public ProductInputType()
        {
            Name = "ProductInput";
            Field<IntGraphType>("id");
            Field<StringGraphType>("name");
            Field<StringGraphType>("description");
            Field<IntGraphType>("availableStock");
            Field<DecimalGraphType>("price");
            Field<ListGraphType<StringGraphType>>("sizes");

            Field<IntGraphType>("productTypeId");
            Field<IntGraphType>("productBrandId");
        }
    }
}
