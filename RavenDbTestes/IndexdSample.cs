using System;
using System.Linq;
using Raven.Client.Documents.Linq;

namespace RavenDbTestes
{
        public class IndexingSample
        {
            public static void Test()
            {
                using (var session = DocumentStoreHolder.Store.OpenSession())
                {
                    var ordersIds = (
                        from order in session.Query<Order>()
                        where order.Company == "companies/1-A"
                        orderby order.OrderedAt
                        select order.Id
                    ).ToList();

                    foreach (var id in ordersIds)
                    {
                        Console.WriteLine(id);
                    }
                }
            }
    }
}