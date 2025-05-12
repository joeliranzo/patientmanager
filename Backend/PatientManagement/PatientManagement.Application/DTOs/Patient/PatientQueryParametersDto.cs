namespace PatientManagement.Application.DTOs.Patient;

public class PatientQueryParametersDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }

    public string? SortBy { get; set; } = "created_date";
    public string? SortOrder { get; set; } = "desc";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
