using FluentValidation;
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
        [FromForm] CreateCandidateRequest request, // Зверни увагу: FromForm, бо ми шлемо файл
        IFormFile? file) // Файл приходить окремо
    {
        // 1. Якщо файл є - парсимо його прямо тут
        if (file != null && file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            // Тобі треба додати _parsingService в конструктор контролера!
            request.ResumeText = await _parsingService.ExtractTextAsync(stream);
        }
    
        // 2. Якщо тексту немає (ані файлу, ані тексту) - помилка
        if (string.IsNullOrWhiteSpace(request.ResumeText))
        {
            return BadRequest("Resume text or PDF file is required.");
        }

        // 3. Віддаємо в сервіс вже заповнений об'єкт
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
        await using var stream = file.OpenReadStream();
    
        // Викликаємо наш новий сервіс
        var text = await _parsingService.ExtractTextAsync(stream);

        // Повертаємо прочитаний текст, щоб ти побачив його очима
        return Ok(new { FileName = file.FileName, ExtractedText = text });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CandidateStatus newStatus)
    {
        try 
        {
            await _candidateService.UpdateStatusAsync(id, newStatus);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}