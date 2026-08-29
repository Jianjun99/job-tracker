using System.ComponentModel.DataAnnotations;

namespace JobTracker.Models;

public enum ApplicationStatus
{
    Saved = 0,
    Applied = 1,
    Interview = 2,
    Offer = 3,
    Rejected = 4
}

public class JobApplication
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Company { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Location { get; set; }

    [StringLength(500)]
    public string? Url { get; set; }

    [StringLength(100)]
    public string? Salary { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Saved;

    [Display(Name = "Matched Skills")]
    [StringLength(300)]
    public string? MatchedSkills { get; set; }

    [Display(Name = "Match Score")]
    public int MatchScore { get; set; }

    [Display(Name = "Track")]
    [StringLength(20)]
    public string? Track { get; set; }

    [Display(Name = "Applied")]
    [DataType(DataType.Date)]
    public DateTime? AppliedDate { get; set; }

    [Display(Name = "Follow up")]
    [DataType(DataType.Date)]
    public DateTime? FollowUpDate { get; set; }

    public string? Notes { get; set; }

    [Display(Name = "Created")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
