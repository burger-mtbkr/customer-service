using Customer.Service.Infrastructure;
using Customer.Service.Models;
using JsonFlatFileDataStore;

namespace Customer.Service.Ignition
{
    public static class DataStoreIgnition
    {
        public static void ConfigureDataStore(this WebApplicationBuilder builder)
        {
            var settings = builder.Configuration
                .GetSection("DatabaseSettings")
                .Get<DatabaseSettings>();

            if(settings == null) throw new ArgumentNullException(nameof(DatabaseSettings));

            var dataStore = new DataStore(
                settings.FilePath,
                settings.UseLowerCamelCase,
                settings.KeyProperty,
                settings.LiveReload,
                settings.EncryptionKey,
                settings.MinifyJson);

            builder.Services.AddSingleton<IDataStore>(d => dataStore);
            builder.Services.AddScoped(s => dataStore.GetCollection<Session>("sessions"));
            builder.Services.AddScoped(s => dataStore.GetCollection<UserModel>("users"));
            builder.Services.AddScoped(s => dataStore.GetCollection<CustomerModel>("customer"));
            builder.Services.AddScoped(s => dataStore.GetCollection<LeadModel>("customer_leads"));
        }
    }
}
