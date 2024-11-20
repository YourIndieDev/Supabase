
using Newtonsoft.Json.Linq;
using System;

public class VectorStore
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public JObject VectorData { get; set; }

    public DateTime CreatedAt { get; set; }
} 