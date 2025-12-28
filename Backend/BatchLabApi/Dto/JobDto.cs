using BatchLabApi.Domain;

namespace BatchLabApi.Dto
{
    public record JobDto
    {
        public string? Id { get; init; }
        public required string? Description { get; init; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; init; }
    }
}