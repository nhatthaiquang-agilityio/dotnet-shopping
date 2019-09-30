using GraphQL.Types;
using dotnet_express_mapper.Services;
using dotnet_express_mapper.GraphQL;

namespace dotnet_express_mapper.Queries
{
    public class APIServiceQuery : ObjectGraphType
    {
        public APIServiceQuery(BookService bookService, ProductService productService, AuthorService authorService)
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
                resolve: context => authorService.GetAuthors()
            );

            Field<AuthorType>(
                "Author",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return authorService.GetAuthor(id);
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

            Field<BookCategoriesType>(
                "BookCategories",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                   var id = context.GetArgument<int>("id");
                   return bookService.GetBookCategory(id);
                }
            );

            Field<CategoryType>(
                "Category",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return bookService.GetCategory(id);
                }
            );

            Field<ListGraphType<SizeType>>(
                "Sizes",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return productService.GetSizeOfProduct(id);
                }
            );
        }
    }
}