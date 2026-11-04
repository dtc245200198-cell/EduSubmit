using System.Net.Http.Json;

var baseAddress = "http://localhost:5000";
using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };

var assignments = await client.GetFromJsonAsync<List<Assignment>>("/api/assignments");

if (assignments is null || assignments.Count == 0)
{
    Console.WriteLine("No assignments found.");
    return;
}

Console.WriteLine("Assignments:");
foreach (var assignment in assignments)
{
    Console.WriteLine($"- {assignment.Id}: {assignment.Title} ({assignment.Description})");
}

var submissionRequest = new
{
    assignmentId = 1,
    studentCode = "SE123",
    repositoryUrl = "https://github.com/demo/repo"
};

var response = await client.PostAsJsonAsync("/api/submissions", submissionRequest);
response.EnsureSuccessStatusCode();

var submission = await response.Content.ReadFromJsonAsync<SubmissionResponse>();

Console.WriteLine();
Console.WriteLine($"Submission created: {submission?.Id}");
Console.WriteLine($"Status: {submission?.Status}");
Console.WriteLine($"ReceivedAt: {submission?.ReceivedAt:O}");

var fetchResponse = await client.GetAsync($"/api/submissions/{submission!.Id}");
Console.WriteLine($"Fetch status: {(int)fetchResponse.StatusCode} {fetchResponse.StatusCode}");

var fetchedSubmission = await fetchResponse.Content.ReadFromJsonAsync<SubmissionResponse>();
Console.WriteLine($"Fetched submission: {fetchedSubmission?.Id}");
Console.WriteLine($"Fetched status: {fetchedSubmission?.Status}");

var missingResponse = await client.GetAsync($"/api/submissions/{Guid.NewGuid()}");
Console.WriteLine($"Missing submission status: {(int)missingResponse.StatusCode} {missingResponse.StatusCode}");

record Assignment(int Id, string Title, string Description);
record SubmissionResponse(Guid Id, int AssignmentId, string StudentCode, string Status, DateTime ReceivedAt);
