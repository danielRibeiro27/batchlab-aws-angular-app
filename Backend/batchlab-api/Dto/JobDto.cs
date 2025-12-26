namespace BatchlabApi.Dto
{
    public record JobDto
    {
        public int Id { get; init; }
        public required string Desc { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}