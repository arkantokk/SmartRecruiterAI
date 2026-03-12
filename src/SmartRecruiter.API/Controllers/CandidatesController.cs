using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Services;
using SmartRecruiter.Domain.Enums;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.API.Controllers;
[ApiController]

[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly CandidateService _candidateService;
    private readonly IValidator<CreateCandidateRequest> _validator;
    private readonly IFileParsingService _parsingService;
    public CandidatesController(CandidateService candidateService, IValidator<CreateCandidateRequest> validator, IFileParsingService parsingService)
    {
        _candidateService = candidateService;
        _validator = validator;
        _parsingService = parsingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] CreateCandidateRequest request, 
        IFormFile? file) 
    {
        if (file != null && file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            request.ResumeText = await _parsingService.ExtractTextAsync(stream);
        }
    
        if (string.IsNullOrWhiteSpace(request.ResumeText))
        {
            return BadRequest("Resume text or PDF file is required.");
        }

        // 3. Віддаємо в сервіс вже заповнений об'єкт
        var id = await _candidateService.RegisterCandidateAsync(request);
        return Ok(id);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCandidates()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }
        var res = await _candidateService.GetCandidatesForUserAsync(userId);
        return Ok(res);
    }
    
    [HttpPost("test-upload")]
    public async Task<IActionResult> TestUpload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не обрано");

        // Відкриваємо потік (не завантажуючи весь файл в пам'ять)
        await using var stream = file.OpenReadStream();
    
        // Викликаємо наш новий сервіс
        var text = await _parsingService.ExtractTextAsync(stream);

        // Повертаємо прочитаний текст, щоб ти побачив його очима
        return Ok(new { FileName = file.FileName, ExtractedText = text });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CandidateStatus newStatus)
    {
            await _candidateService.UpdateStatusAsync(id, newStatus);
            return NoContent();
    }
}