namespace SmartRecruiter.Application.DTOs;

public record PagedResponse<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
    );