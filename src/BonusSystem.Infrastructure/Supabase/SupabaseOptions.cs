namespace BonusSystem.Infrastructure.Supabase;

public class SupabaseOptions
{
    public const string Position = "Supabase";

    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string JwtSecret { get; set; } = string.Empty;
}