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
    private readonly IFileParsingService _parsingService;
    public CandidatesController(CandidateService candidateService, IValidator<CreateCandidateRequest> validator, IFileParsingService parsingService)
    {
        _candidateService = candidateService;
        _validator = validator;
        _parsingService = parsingService;
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
    
    [HttpPost("test-upload")]
    public async Task<IActionResult> TestUpload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не обрано");

        // Відкриваємо потік (не завантажуючи весь файл в пам'ять)
        using var stream = file.OpenReadStream();
    
        // Викликаємо наш новий сервіс
        var text = await _parsingService.ExtractTextAsync(stream);

        // Повертаємо прочитаний текст, щоб ти побачив його очима
        return Ok(new { FileName = file.FileName, ExtractedText = text });
    }
}