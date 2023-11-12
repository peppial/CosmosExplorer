using System;
using Newtonsoft.Json.Linq;

namespace CosmosExplorer.Core.Query
{
	public record QueryResultModel<T>
	{
        public double RequestCharge { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public T? Items { get; set; }
        public Exception? Error { get; set; }
        public string? ContinuationToken { get; set; }
        public IEnumerable<string> Warnings { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public bool HasMore => !string.IsNullOrEmpty(ContinuationToken);
        public JObject? Diagnostics { get; internal set; }
        public string? IndexMetrics { get; internal set; }
    }
}

