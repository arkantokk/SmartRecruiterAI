using Microsoft.AspNetCore.Mvc;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Services;

namespace SmartRecruiter.API.Controllers;
[ApiController]
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
        var id = await _jobVacancyService.AddJobVacancyAsync(jobVacancyRequest);
        return Ok(id);
    }
}