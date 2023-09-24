using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;

namespace CosmosExplorer.Domain.Models
{
    public interface IDocumentModel
    {
        string? Id { get; }
        string? ETag { get; }
        string? SelfLink { get; }
        string? Attachments { get; }
        long TimeStamp { get; }
        string? PartitionKey { get; }
        bool HasPartitionKey { get; }
    }

    public class DocumentModel : IDocumentModel
    {

        public static DocumentModel CreateFrom(JObject resource, string? partitionPath)
        {
            var instance = resource.ToObject<DocumentModel>();

            if (instance is null)
            {
                throw new Exception("Cannot create CosmosDocument");
            }

           /* if (partitionPath is not null)
            {
                var doc = new DocumentModel(resource, partitionPath);

                if (doc.PK is not null)
                {
                    instance.HasPartitionKey = true;
                    instance.PartitionKey = doc.PK;
                }
            }*/

            return instance;
        }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("_etag")]
        public string? ETag { get; set; }

        [JsonProperty("_self")]
        public string? SelfLink { get; set; }

        [JsonProperty("_attachments")]
        public string? Attachments { get; set; }

        [JsonProperty("_ts")]
        public long TimeStamp { get; set; }

        [JsonProperty("_partitionKey")]
        public string? PartitionKey { get; set; }

        [JsonProperty("_hasPartitionKey")]
        public bool HasPartitionKey { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as IDocumentModel);
        }

        public bool Equals(IDocumentModel? other)
        {
            return other != null
                    && Id == other.Id
                    && PartitionKey == other.PartitionKey;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, SelfLink, PartitionKey);
        }
    }
}

