using GraphQL;
using GraphQL.Types;

namespace dotnet_express_mapper.Queries
{
    public class GraphQLSchema : Schema
    {
        public GraphQLSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<APIServiceQuery>();
            Mutation = resolver.Resolve<ProductMutation>();
        }
    }
}