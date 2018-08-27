using System;
using Raven.Client.Documents;
using static System.Console;
using System.Linq;

namespace RavenDbTestes
{
    class Program
    {
        static void Main(string[] args)
        {
           //Run();
//            new ProgramCrud().Run();
//            IndexingSample.Test();
//            new Index_Employees_ByFirstAndLastName().Execute(DocumentStoreHolder.Store);
//            new IndexSampleByCode().Run();
//            var multipleIndex = new MultipleIndex.ProgramMultMapINdex();
//            multipleIndex.Run();
            
//            var mapReduce = new MapReduce1.Runner();            
//            mapReduce.Execute();
//            MapReduce2.Runner.Execute();
            
            RavenDbTestes.MapReduce1.Runner.Execute();
            
            WriteLine(new string('-', 100));
            WriteLine("Digite um linha pra finalizar !!");
            ReadLine();
        }

        static void Run()
        {
            while (true)
            {
                WriteLine("Please, enter a company id (0 to exit): ");

                var companyId = ReadLine();

                if (companyId == "0") 
                    break;

//                QueryCompanyOrders(Convert.ToInt32(companyId));
                QueryCompanyOrdersRql(Convert.ToInt32(companyId));
                
            }

            WriteLine("Goodbye!");

            //Unit 1 - Lesso 5
            //while (true)
            //{
            //    WriteLine("Please, enter an order # (0 to exit): ");

            //    int orderNumber;
            //    if (!int.TryParse(ReadLine(), out orderNumber))
            //    {
            //        WriteLine("Order # is invalid.");
            //        continue;
            //    }

            //    if (orderNumber == 0) break;

            //    PrintOrder(orderNumber);
            //}

            //using(var session = DocumentStoreHolder.Store.OpenSession()){

            //    var p = session.Load<Product>("products/1-A");

            //    Console.WriteLine(p.Name);
            //}   

            //Console.WriteLine("Hello World!");
        }
        
        private static void QueryCompanyOrdersRql(int companyId)
        {
            var chaveDaEmpresa = $"companies/{companyId}-A";
            
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var orders = session.Advanced.RawQuery<Order>(
                    "from Orders where Company == $companyId include Company");

                orders.AddParameter("$companyId", chaveDaEmpresa);

                var company = session.Load<Company>(chaveDaEmpresa);

                if (company == null)
                {
                    WriteLine("Company not found.");
                    return;
                }

                WriteLine($"Orders for {company.Name}");

                foreach (var order in orders)
                {
                    WriteLine($"{order.Id} - {order.OrderedAt}");
                }
            }
        }

        private static void QueryCompanyOrders(int companyId)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var orders = session.Query<Order>()
                                    .Include(o => o.Company)
                                    .Where(x => x.Company == $"companies/{companyId}-A")
                                    .Select(x => x)
                                    .ToList();

                var company = session.Load<Company>($"companies/{companyId}-A");

                //if (company == null)
                //{
                //    WriteLine("Company not found.");
                //    return;
                //}

                //WriteLine($"Orders for {company.Name}");

                foreach (var order in orders)
                {
                    WriteLine($"{order.Company} - {order.Id} - {order.OrderedAt}");
                }
            }
        }

        private static void PrintOrder(int orderNumber)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var order = session
                    .Include<Order>(o => o.Company)
                    .Include(o => o.Employee)
                    .Include(o => o.Lines.Select(l => l.Product))
                    .Load($"orders/{orderNumber}-A");

                if (order == null)
                {
                    WriteLine($"Order #{orderNumber} not found.");
                    return;
                }

                WriteLine($"Order #{orderNumber}");

                var c = session.Load<Company>(order.Company);
                WriteLine($"Company : {c.Id} - {c.Name}");

                var e = session.Load<Employee>(order.Employee);
                WriteLine($"Employee: {e.Id} - {e.LastName}, {e.FirstName}");

                foreach (var orderLine in order.Lines)
                {
                    var p = session.Load<Product>(orderLine.Product);
                    WriteLine($"   - {orderLine.ProductName}," +
                              $" {orderLine.Quantity} x {p.QuantityPerUnit}");
                }
            }
        }
    }


}
