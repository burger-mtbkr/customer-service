namespace Customer.Service.Infrastructure
{
    public record DatabaseSettings
    {
        public string DefaultDbName { get; init; }

        public bool UseLowerCamelCase { get; init; }

        public string KeyProperty { get; init; }

        public string EncryptionKey { get; init; }

        public bool MinifyJson { get; init; }
      
        public string FilePath => Path.Combine(AppContext.BaseDirectory, DefaultDbName);
        
        public bool LiveReload => FilePath != DefaultDbName;
    }
}
