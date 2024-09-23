namespace urbanmart.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ProductsCollectionName { get; set; }
        public string OrdersCollectionName { get; set; }

        public string UsersCollectionName {get; set;}
        public string InventoryCollectionName {get; set;}
        public string VendorFeedbackCollectionName {get; set;}
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string ProductsCollectionName { get; set; }
        string OrdersCollectionName { get; set; }
        string UsersCollectionName {get; set;}
        string InventoryCollectionName {get; set;}
        string VendorFeedbackCollectionName {get; set;}

        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
