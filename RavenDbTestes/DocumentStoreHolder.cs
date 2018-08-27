using System;
using System.Reflection;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace RavenDbTestes
{
    public class DocumentStoreHolder
    {
        private static readonly Lazy<IDocumentStore> LazyStore = new Lazy<IDocumentStore>(() =>
        {
            var documentStore = new DocumentStore()
            {
                Urls = new[] { "http://localhost:8080" },
                Database = "Teste"
            };

            documentStore.Initialize();

            var asm = Assembly.GetExecutingAssembly();
            IndexCreation.CreateIndexes(asm, documentStore);

            return documentStore;
        });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
