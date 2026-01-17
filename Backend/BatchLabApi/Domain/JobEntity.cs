using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace BatchLabApi.Domain
{
    public class JobEntity
    {
        public Guid Id { get; set; }
        public string? Description { get; set; } //TO-DO: Validate null or empty
        public string? Status { get; set; } //TO-DO: Validate null or empty
        public DateTime CreatedAt { get; set; }

        public JobEntity()
        {
        }

        public JobEntity(string description)
        {
            if(string.IsNullOrEmpty(description))
                throw new ArgumentException("Job description cannot be null or empty.", nameof(description));

            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = "Pending";
            Description = description;
        }
    }
}