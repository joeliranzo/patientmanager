using Microsoft.AspNetCore.Mvc;

namespace PatientManagement.Application.DTOs.Patient;

public class PatientQueryParametersDto
{
    [FromQuery(Name = "first_name")]
    public string? FirstName { get; set; }

    [FromQuery(Name = "last_name")]
    public string? LastName { get; set; }

    [FromQuery(Name = "email")]
    public string? Email { get; set; }

    [FromQuery(Name = "sort_by")]
    public string? SortBy { get; set; } = "created_date";

    [FromQuery(Name = "sort_order")]
    public string? SortOrder { get; set; } = "desc";

    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "page_size")]
    public int PageSize { get; set; } = 10;
}
