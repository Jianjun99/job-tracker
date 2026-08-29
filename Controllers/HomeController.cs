using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobTracker.Data;
using JobTracker.Models;

namespace JobTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var apps = _context.JobApplications.AsNoTracking();

        var model = new DashboardViewModel
        {
            Total = await apps.CountAsync(),
            ByStatus = await apps.GroupBy(a => a.Status)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count),
            FollowUpsDue = await apps
                .Where(a => a.FollowUpDate != null
                            && a.FollowUpDate <= DateTime.UtcNow.Date.AddDays(7)
                            && a.Status != ApplicationStatus.Rejected
                            && a.Status != ApplicationStatus.Offer)
                .OrderBy(a => a.FollowUpDate)
                .Take(10)
                .ToListAsync(),
            TopMatches = await apps
                .Where(a => a.Status == ApplicationStatus.Saved)
                .OrderByDescending(a => a.MatchScore)
                .Take(5)
                .ToListAsync()
        };

        // Response rate: of everything actually applied to, how many
        // progressed to an interview or offer.
        var applied = model.ByStatus.GetValueOrDefault(ApplicationStatus.Applied)
                      + model.ByStatus.GetValueOrDefault(ApplicationStatus.Interview)
                      + model.ByStatus.GetValueOrDefault(ApplicationStatus.Offer)
                      + model.ByStatus.GetValueOrDefault(ApplicationStatus.Rejected);
        var responded = model.ByStatus.GetValueOrDefault(ApplicationStatus.Interview)
                        + model.ByStatus.GetValueOrDefault(ApplicationStatus.Offer)
                        + model.ByStatus.GetValueOrDefault(ApplicationStatus.Rejected);
        model.ResponseRate = applied == 0 ? 0 : Math.Round(100.0 * responded / applied);

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
