using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;

namespace CosmosExplorer.Core.Models
{
    public interface IDocumentModel
    {
        string? Id { get; }
        string? ETag { get; }
        string? SelfLink { get; }
        string? Attachments { get; }
        long TimeStamp { get; }
        string? PartitionKey1 { get; }
        string? PartitionKey2 { get; }
        string? PartitionKey3 { get; }
        bool HasPartitionKey { get; }

        Partition Partition => new Partition(PartitionKey1, PartitionKey2, PartitionKey3);
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

        [JsonProperty("_partitionKey1")]
        public string? PartitionKey1 { get; set; }
        
        [JsonProperty("_partitionKey2")]
        public string? PartitionKey2 { get; set; }
        
        [JsonProperty("_partitionKey3")]
        public string? PartitionKey3 { get; set; }

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
                    && PartitionKey1 == other.PartitionKey1
                    && PartitionKey2 == other.PartitionKey2
                    && PartitionKey3 == other.PartitionKey3;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, SelfLink, PartitionKey1, PartitionKey2, PartitionKey3);
        }
    }
}

