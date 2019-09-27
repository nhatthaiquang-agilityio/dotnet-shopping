using System;
using System.Collections.Generic;
using System.Reflection;
using dotnet_express_mapper.Data;
using dotnet_express_mapper.GraphQL;
using dotnet_express_mapper.Models;
using dotnet_express_mapper.Queries;
using dotnet_express_mapper.Services;
using ExpressMapper;
using GraphiQl;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotnet_express_mapper
{
    public class Startup
    {
        public const string GraphQlPath = "/graphql";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            MappingRegistration();

            // Add framework services.
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration["ConnectionString"], sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                }));

            services.AddScoped<BookService>();
            services.AddScoped<ProductService>();

            InitData(services);

            // using for graphql schema and mutation
            services.AddSingleton<ProductInputType>();
            services.AddSingleton<BookType>();
            services.AddSingleton<AuthorType>();
            services.AddSingleton<CategoryType>();
            services.AddSingleton<BookCategoriesType>();
            services.AddSingleton<GraphQL.ProductType>();
            services.AddSingleton<SizeType>();
            services.AddSingleton<APIServiceQuery>();
            services.AddSingleton<ProductMutation>();

            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(new GraphQLSchema(new FuncDependencyResolver(sp.GetService)));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseGraphiQl(GraphQlPath);
            app.UseMvc();
        }

        private void MappingRegistration()
        {
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Register<Product, ProductViewModel>()
                .Function(dest => dest.Sizes, src =>
                {
                    List<string> sizes = new List<string>(src.Sizes.Count);
                    foreach (var size in src.Sizes)
                    {
                        if (size != null)
                        {
                            sizes.Add(size.Name);
                        }
                    }
                    return sizes;
                });

            Mapper.Register<Book, BookViewModel>()
                .Member(dest => dest.Author, src => src.Author.FirstName + " "+ src.Author.LastName)
                .Function(dest => dest.BookCategories, src => {
                    List<string> categories = new List<string>(src.BookCategories.Count);
                    foreach (var bookCategory in src.BookCategories)
                    {
                        if (bookCategory != null && bookCategory.Category != null)
                        {
                            categories.Add(bookCategory.Category.CategoryName);
                        }
                    }
                    return categories;
                });
            Mapper.Register<Author, AuthorDTO>();
            Mapper.Register<BookCategory, BookCategoryViewModel>();
            Mapper.Register<Category, CategoryViewModel>();
            Mapper.Compile();
        }

        private void InitData(IServiceCollection services)
        {
            ServiceProvider sp = services.BuildServiceProvider();
            var context = sp.GetRequiredService<AppDbContext>();

            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Migration Database");
                Console.WriteLine(e);
            }

            new AppContextSeed().SeedAsync(context).Wait();
        }
    }
}
