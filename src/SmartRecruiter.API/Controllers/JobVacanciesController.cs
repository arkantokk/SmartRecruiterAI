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
    private readonly CandidateService _candidateService;

    public JobVacanciesController(JobVacancyService jobVacancyService, CandidateService candidateService)
    {
        _jobVacancyService = jobVacancyService;
        _candidateService = candidateService;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVacancyById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var vacancy = await _jobVacancyService.GetVacancyByIdAsync(id, userId);
        return Ok(vacancy);
    }
    
    [HttpGet("{id:guid}/candidates")]
    public async Task<IActionResult> GetCandidatesByVacancyId(Guid id, int pageNumber, int pageSize)
    {
        var candidates = await _candidateService.GetAllCandidatesByVacancyIdAsync(id, pageNumber, pageSize);
        return Ok(candidates);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateJobVacancy(Guid id, [FromBody] UpdateJobVacancyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        try 
        {
            await _jobVacancyService.UpdateVacancyAsync(id, userId, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}