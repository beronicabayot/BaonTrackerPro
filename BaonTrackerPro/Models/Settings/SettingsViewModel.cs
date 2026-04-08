namespace BaonTrackerPro.Models.Settings
{
    public class SettingsViewModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public string ProfileImageUrl { get; set; } = "";

        // Optional fields for later (UI already supports them)
        public string? DateOfBirthLabel { get; set; }
        public string? GenderLabel { get; set; }
    }
}

