using FluentValidation;
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
    private readonly IValidator<CreateCandidateRequest> _validator;
    public CandidatesController(CandidateService candidateService, IValidator<CreateCandidateRequest> validator)
    {
        _candidateService = candidateService;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCandidate([FromBody] CreateCandidateRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var id = await _candidateService.RegisterCandidateAsync(request);
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> GetCandidates()
    {
        var res = await _candidateService.GetAllCandidatesAsync();
        return Ok(res);
    }
}