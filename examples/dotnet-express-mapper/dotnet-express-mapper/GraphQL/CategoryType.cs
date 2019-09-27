using GraphQL.Types;
using dotnet_express_mapper.Models;
namespace dotnet_express_mapper.GraphQL
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType()
        {
            Field(x => x.CategoryId).Description("Id");
            Field(x => x.CategoryName).Description("Name");
        }
    }
}
