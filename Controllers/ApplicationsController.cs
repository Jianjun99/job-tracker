using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobTracker.Data;
using JobTracker.Models;

namespace JobTracker.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ApplicationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Applications?status=2&search=.net&track=fullstack
    public async Task<IActionResult> Index(ApplicationStatus? status, string? search,
        string? track)
    {
        var query = _context.JobApplications.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }
        if (!string.IsNullOrWhiteSpace(track))
        {
            query = query.Where(a => a.Track == track);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a =>
                a.Title.Contains(term) ||
                a.Company.Contains(term) ||
                (a.MatchedSkills != null && a.MatchedSkills.Contains(term)));
        }

        ViewData["StatusFilter"] = status;
        ViewData["Search"] = search;
        ViewData["TrackFilter"] = track;

        var items = await query
            .OrderByDescending(a => a.MatchScore)
            .ThenBy(a => a.FollowUpDate ?? DateTime.MaxValue)
            .ToListAsync();
        return View(items);
    }

    // GET: Applications/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var app = await _context.JobApplications.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
        return app == null ? NotFound() : View(app);
    }

    // GET: Applications/Create
    public IActionResult Create() => View();

    // POST: Applications/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,Title,Company,Location,Url,Salary,Status,MatchedSkills,MatchScore,Track,AppliedDate,FollowUpDate,Notes")]
        JobApplication jobApplication)
    {
        if (!ModelState.IsValid) return View(jobApplication);
        jobApplication.CreatedAt = DateTime.UtcNow;
        _context.Add(jobApplication);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Applications/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var app = await _context.JobApplications.FindAsync(id);
        return app == null ? NotFound() : View(app);
    }

    // POST: Applications/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Title,Company,Location,Url,Salary,Status,MatchedSkills,MatchScore,Track,AppliedDate,FollowUpDate,Notes,CreatedAt")]
        JobApplication jobApplication)
    {
        if (id != jobApplication.Id) return NotFound();
        if (!ModelState.IsValid) return View(jobApplication);
        try
        {
            _context.Update(jobApplication);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.JobApplications.AnyAsync(a => a.Id == id))
                return NotFound();
            throw;
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Applications/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var app = await _context.JobApplications.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
        return app == null ? NotFound() : View(app);
    }

    // POST: Applications/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var app = await _context.JobApplications.FindAsync(id);
        if (app != null)
        {
            _context.JobApplications.Remove(app);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Applications/QuickStatus/5?status=1
    // Lets the user advance a status straight from the index table.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuickStatus(int id, ApplicationStatus status)
    {
        var app = await _context.JobApplications.FindAsync(id);
        if (app == null) return NotFound();
        app.Status = status;
        if (status >= ApplicationStatus.Applied && app.AppliedDate == null)
        {
            app.AppliedDate = DateTime.UtcNow.Date;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
