using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Customer.Service.Ignition
{
    public static class DataProtectionIgnition
    {
        public static void ConfigureDataProtection(this WebApplicationBuilder builder)
        {

            var keysPath = builder.Configuration["Platform:Data_Protection_Keys_Path"];
            DirectoryInfo directoryInfo = new (keysPath);

            // Would be beter to save the keys to something like AWS param store
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(directoryInfo)                
                 .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                 {
                     EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                     ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                 });
        }
    }
}
