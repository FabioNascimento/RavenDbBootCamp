using System;
using System.Linq;
using Raven.Client.Documents.Linq;

namespace RavenDbTestes
{
    public class IndexSampleByCode
    {
        public void Run()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var results = session
                    .Query<Employee, Index_Employees_ByFirstAndLastName>()
                    .Where(x => x.FirstName == "Robert")
                    .ToList();

                foreach (var employee in results)
                {
                    Console.WriteLine($"{employee.LastName}, {employee.FirstName}");
                }
            }
        }
    }
}