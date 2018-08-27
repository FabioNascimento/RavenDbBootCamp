using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

namespace RavenDbTestes.MultipleIndex
{
    public class Employee
    {
        public string Id        { get; set; }
        public string FirstName { get; set; }
        public string LastName  { get; set; }
    }

    public class Contact
    {
        public string Name { get; set; }
    }

    public class Company
    {
        public string  Id      { get; set; }
        public Contact Contact { get; set; }
    }

    public class Supplier
    {
        public string  Id      { get; set; }
        public Contact Contact { get; set; }
    }
    
    public class People_Search :
        AbstractMultiMapIndexCreationTask<People_Search.Result>
    {
        public class Result
        {
            public string SourceId { get; set; }
            public string Name     { get; set; }
            public string Type     { get; set; }
        }

        public People_Search()
        {
            AddMap<Company>(companies =>
                from company in companies
                select new Result
                {
                    SourceId = company.Id,
                    Name     = company.Contact.Name,
                    Type     = "Company's contact"
                }
            );

            AddMap<Supplier>(suppliers =>
                from supplier in suppliers
                select new Result
                {
                    SourceId = supplier.Id,
                    Name     = supplier.Contact.Name,
                    Type     = "Supplier's contact"
                }
            );

            AddMap<Employee>(employees =>
                from employee in employees
                select new Result
                {
                    SourceId = employee.Id,
                    Name     = $"{employee.FirstName} {employee.LastName}",
                    Type     = "Employee"
                }
            );

            Index(entry => entry.Name, FieldIndexing.Search);

            Store(entry => entry.SourceId, FieldStorage.Yes);
            Store(entry => entry.Name,     FieldStorage.Yes);
            Store(entry => entry.Type,     FieldStorage.Yes);
        }
    }


    public class Runnner
    {
        
        public void Execute()
        {
            
            Console.Title = "Multi-map sample";
            
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                while (true)
                {
                    Console.Write("\nSearch terms: ");
                    
                    var searchTerms = Console.ReadLine();

                    foreach (var result in Search(session, searchTerms))
                    {
                        Console.WriteLine($"{result.SourceId}\t{result.Type}\t{result.Name}");
                    }
                }
            }
        }
        
        public static IEnumerable<People_Search.Result> Search(
            IDocumentSession session,
            string           searchTerms
        )
        {
            var results = session.Query<People_Search.Result, People_Search>()
                .Search(
                    r => r.Name,
                    searchTerms,
                    Decimal.One,
                    SearchOptions.Or
                )
                .ProjectInto<People_Search.Result>()
                .ToList();

            return results;
        }
    }
    
}