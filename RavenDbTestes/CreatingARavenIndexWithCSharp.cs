namespace RavenDbTestes
{
    using Raven.Client.Documents.Indexes;

    public class Index_Employees_ByFirstAndLastName : AbstractIndexCreationTask<Employee>
    {
        public override string IndexName => "Employees/ByFirstAndLastName";

        public override IndexDefinition CreateIndexDefinition()
        {
            return new IndexDefinition
            {
                Maps =
                {
                    @"from doc in docs.Employees
select new
{
    FirstName = doc.FirstName,
    LastName = doc.LastName
}"
                }
            };
        }
    }
}