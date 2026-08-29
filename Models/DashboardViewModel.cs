using JobTracker.Models;

namespace JobTracker.Models;

public class DashboardViewModel
{
    public int Total { get; set; }
    public Dictionary<ApplicationStatus, int> ByStatus { get; set; } = new();
    public double ResponseRate { get; set; }
    public List<JobApplication> FollowUpsDue { get; set; } = new();
    public List<JobApplication> TopMatches { get; set; } = new();

    public int Count(ApplicationStatus status) => ByStatus.GetValueOrDefault(status);
}
