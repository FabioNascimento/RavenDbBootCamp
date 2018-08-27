using System;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace RavenDbTestes.MapReduce1
{
    public class Category
    {
        public string Name { get; set; }
    }

    public class Product
    {
        public string Category { get; set; }
    }
    
    public class Products_ByCategory :
        AbstractIndexCreationTask<Product, Products_ByCategory.Result>
    {
        public class Result
        {
            public string Category { get; set; }
            public int    Count    { get; set; }
        }

        public Products_ByCategory()
        {
            Map = products =>
                from product in products
                let categoryName = LoadDocument<RavenDbTestes.Category>(product.Category).Name
                select new
                {
                    Category = categoryName,
                    Count    = 1
                };

            Reduce = results =>
                from result in results                
                group result by result.Category into g
                select new
                {
                    Category = g.Key,
                    Count    = g.Sum(x => x.Count)
                };
        }
    }

    public class Runner
    {
        public static void Execute()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var results = session
                    .Query<Products_ByCategory.Result, Products_ByCategory>()
//                    .Include(x => x.Category)
                    .ToList();

                foreach (var result in results)
                {
//                    var category = session.Load<Category>(result.Category);
                    Console.WriteLine($"{result.Category} has {result.Count} items.");
                }
            }
        }
    }
    
}