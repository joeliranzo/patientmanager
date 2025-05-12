﻿namespace PatientManagement.Application.DTOs.Shared;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
    public int TotalPages =>
        PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}

