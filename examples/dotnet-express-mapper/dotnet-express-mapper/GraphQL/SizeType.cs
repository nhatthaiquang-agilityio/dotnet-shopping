using GraphQL.Types;
using dotnet_express_mapper.Models;

namespace dotnet_express_mapper.GraphQL
{
    public class SizeType : ObjectGraphType<Size>
    {
        public SizeType()
        {
            Field(x => x.Id).Description("Id");
            Field(x => x.Name).Description("Name ");
            Field(x => x.Code).Description("COde");
            Field(x => x.ProductId).Description("Product Id");
        }
    }
}
