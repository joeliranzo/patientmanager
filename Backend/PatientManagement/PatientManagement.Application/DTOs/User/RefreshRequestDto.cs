using System.Text.Json.Serialization;

namespace PatientManagement.Application.DTOs.User;

public class RefreshRequestDto
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = null!;
}