using GraphQL.Types;
using dotnet_express_mapper.Models;

namespace dotnet_express_mapper.GraphQL
{
    public class BookCategoriesType : ObjectGraphType<BookCategory>
    {
        public BookCategoriesType()
        {
            Field(x => x.BookId).Description("Book Id");
            Field(x => x.CategoryId).Description("Category Id");
            Field(x => x.Category, type: typeof(CategoryType)).Description("Category");
        }
    }
}