using System;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace RavenDbTestes.MapReduce2
{
    public class Order {
        public string   Employee  { get;  }
        public DateTime OrderedAt { get; }
    }

    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName  { get; set; }
    }
    
    public class Employees_SalesPerMonth :
        AbstractIndexCreationTask<Order, Employees_SalesPerMonth.Result>
    {
        public class Result
        {
            public string Employee   { get; set; }
            public string Month      { get; set; }
            public int    TotalSales { get; set; }
        }

        public Employees_SalesPerMonth()
        {
            Map = orders =>
                from order in orders
                select new
                {
                    order.Employee,
                    Month      = order.OrderedAt.ToString("yyyy-MM"),
                    TotalSales = 1
                };

            Reduce = results =>
                from result in results
                group result by new
                {
                    result.Employee,
                    result.Month
                }
                into g
                select new
                {
                    g.Key.Employee,
                    g.Key.Month,
                    TotalSales = g.Sum(x => x.TotalSales)
                };
        }
    }
    
    public class Runner
    {
        public static void Execute()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var query = session
                    .Query<Employees_SalesPerMonth.Result, Employees_SalesPerMonth>()
                    .Include(x => x.Employee);

                var results = (
                    from result in query
                    where result.Month == "1998-03"
                    orderby result.TotalSales descending
                    select result
                ).ToList();

                foreach (var result in results)
                {
                    var employee = session.Load<Employee>(result.Employee);
                    Console.WriteLine(
                        $"{employee.FirstName} {employee.LastName} made {result.TotalSales} sales.");
                }
            }
        }
    }
}