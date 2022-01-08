using System;

namespace SimpleSplit.Domain.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public Type EntityType { get; }
        public string EntityAsJson { get; }
        public string ProviderMessage { get; set; }

        public DuplicateKeyException(Type entityType,
            string entityAsJson,
            string providerMessage,
            string message,
            Exception innerException) : base(message, innerException)
        {
            EntityType = entityType;
            EntityAsJson = entityAsJson;
            ProviderMessage = providerMessage;
        }
    }
}