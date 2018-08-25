using System;
using Raven.Client.Documents;

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

            return documentStore.Initialize();
        });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
