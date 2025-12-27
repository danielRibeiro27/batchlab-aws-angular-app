using BatchLabApi.Domain;

namespace BatchLabApi.Dto
{
    public record JobDto
    {
        public int? Id { get; init; }
        public required string Description { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}