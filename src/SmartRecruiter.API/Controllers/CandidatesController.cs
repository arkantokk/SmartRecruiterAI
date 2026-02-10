using Microsoft.AspNetCore.Mvc;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Services;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly CandidateService _candidateService;

    public CandidatesController(CandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCandidate([FromBody] CreateCandidateRequest request)
    {
        var id = await _candidateService.RegisterCandidateAsync(request);
        return Ok(id);
    }
}