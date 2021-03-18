using System;
namespace OfferConfigurator.Databases
{
    public class OfferConfiguratorDatabaseSettings : IOfferConfiguratorDatabaseSettings
    {
        public string ProductsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IOfferConfiguratorDatabaseSettings
    {
        string ProductsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}

