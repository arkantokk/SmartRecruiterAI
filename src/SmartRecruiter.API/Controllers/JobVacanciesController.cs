using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Services;

namespace SmartRecruiter.API.Controllers;
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JobVacanciesController : ControllerBase
{
    private readonly JobVacancyService _jobVacancyService;

    public JobVacanciesController(JobVacancyService jobVacancyService)
    {
        _jobVacancyService = jobVacancyService;
    }

    [HttpPost]
    public async Task<ActionResult> AddJobAsync(CreateJobVacancyRequest jobVacancyRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) 
        {
            return Unauthorized("User ID not found in token.");
        }
        var id = await _jobVacancyService.AddJobVacancyAsync(jobVacancyRequest, userId);
        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult> GetUserVacancies()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var vacancies = await _jobVacancyService.GetUserVacanciesAsync(userId);
        return Ok(vacancies);
    }
}