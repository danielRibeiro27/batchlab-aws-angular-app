using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace BatchLabApi.Domain
{
    public class JobEntity()
    {
        public Guid Id { get; set; }
        public string? Description { get; set; } //TO-DO: Validate null or empty
        public string? Status { get; set; } //TO-DO: Validate null or empty
        public DateTime CreatedAt { get; set; }

        public JobEntity New(string description)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = "Pending";
            Description = description;
            return this;
        }
    }
}