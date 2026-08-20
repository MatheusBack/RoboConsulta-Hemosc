namespace RoboConsulta.Models;

public class BloodSearchLog
{
    public int Id { get; set; }
    public required string BloodType { get; set; }
    public required string Status { get; set; }
    public DateTime SearchedAt { get; set; }
}
