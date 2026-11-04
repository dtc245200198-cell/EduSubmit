var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var assignments = new[]
{
    new AssignmentDto(1, "Bài tập 1", "Xây dựng API đầu tiên."),
    new AssignmentDto(2, "Bài tập 2", "Xử lý dữ liệu và validation.")
};

var submissions = new List<SubmissionDto>();

app.MapGet("/", () => new { service = "Submission.Api", status = "running" });

app.MapGet("/api/assignments", () => Results.Ok(assignments));

app.MapGet("/api/submissions/{id:guid}", (Guid id) =>
{
    var submission = submissions.FirstOrDefault(s => s.Id == id);
    return submission is null ? Results.NotFound() : Results.Ok(submission);
});

app.MapPost("/api/submissions", (SubmissionCreateRequest request) =>
{
    if (request.AssignmentId <= 0 || string.IsNullOrWhiteSpace(request.StudentCode) || string.IsNullOrWhiteSpace(request.RepositoryUrl))
    {
        return Results.BadRequest(new { message = "Thiếu dữ liệu hoặc assignmentId <= 0." });
    }

    var submission = new SubmissionDto
    {
        Id = Guid.NewGuid(),
        AssignmentId = request.AssignmentId,
        StudentCode = request.StudentCode,
        Status = "received",
        ReceivedAt = DateTime.UtcNow
    };

    submissions.Add(submission);

    return Results.Created($"/api/submissions/{submission.Id}", submission);
});

app.Run();

record AssignmentDto(int Id, string Title, string Description);

record SubmissionCreateRequest
{
    public int AssignmentId { get; init; }
    public string StudentCode { get; init; } = string.Empty;
    public string RepositoryUrl { get; init; } = string.Empty;
}

record SubmissionDto
{
    public Guid Id { get; init; }
    public int AssignmentId { get; init; }
    public string StudentCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime ReceivedAt { get; init; }
};
