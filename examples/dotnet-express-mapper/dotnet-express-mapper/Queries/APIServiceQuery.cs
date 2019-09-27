using GraphQL.Types;
using dotnet_express_mapper.Services;
using dotnet_express_mapper.GraphQL;
using dotnet_express_mapper.Data;
using System.Linq;

namespace dotnet_express_mapper.Queries
{
    public class APIServiceQuery : ObjectGraphType
    {
        public APIServiceQuery(BookService bookService, ProductService productService, AppDbContext appDbContextdb)
        {
            Field<BookType>(
                "Book",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return bookService.GetBook(id);
                }
            );

            Field<ListGraphType<BookType>>(
                "Books",
                resolve: context => bookService.GetBooks()
            );


            Field<ListGraphType<AuthorType>>(
                "Authors",
                resolve: context => appDbContextdb.Authors
            );

            Field<AuthorType>(
                "Author",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "The ID of the Author." }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return appDbContextdb.Authors.FirstOrDefault(i => i.Id == id);
                }
            );

            Field<BookCategoriesType>(
                "BookCategories",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "The ID of the Author." }),
                resolve: context =>
                {
                   var id = context.GetArgument<int>("id");
                   return appDbContextdb.BookCategories.FirstOrDefault(i => i.BookId == id);
                }
            );

            Field<CategoryType>(
                "Catgeory",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "The ID of the Author." }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return appDbContextdb.Categories.FirstOrDefault(i => i.CategoryId == id);
                }
            );


            Field<ProductType>(
                "Product",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return productService.GetProduct(id);
                }
            );

            Field<ListGraphType<ProductType>>(
                "Products",
                resolve: context => productService.GetProducts()
            );

            Field<SizeType>(
                "Size",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "The ID of the Author." }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return appDbContextdb.Sizes.FirstOrDefault(i => i.ProductId == id);
                }
            );
        }
    }
}