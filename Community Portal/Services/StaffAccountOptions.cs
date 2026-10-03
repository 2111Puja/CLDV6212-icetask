namespace Community_Portal.Services
{
    /// <summary>
    /// Municipality staff login. Override on Render with the environment variables
    /// Staff__Username, Staff__Password and Staff__DisplayName. The defaults exist so the lecturer can
    /// log in to a freshly deployed demo; change them for any real use.
    /// </summary>
    public class StaffAccountOptions
    {
        public string Username { get; set; } = "staff@municipality.gov.za";
        public string Password { get; set; } = "Municipal@2026";
        public string DisplayName { get; set; } = "Municipal Staff";
    }

    public static class ReferenceGenerator
    {
        /// <summary>Creates a reference such as RPT-2026-483920.</summary>
        public static string Generate() =>
            $"RPT-{DateTime.UtcNow:yyyy}-{System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000)}";
    }

    public static class ImageValidator
    {
        public const long MaxBytes = 4 * 1024 * 1024;

        /// <summary>Checks the file signature (magic bytes) rather than trusting the client's content type.</summary>
        public static string? DetectContentType(byte[] data)
        {
            if (data.Length < 12) return null;
            if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF) return "image/jpeg";
            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47) return "image/png";
            if (data[0] == 'G' && data[1] == 'I' && data[2] == 'F' && data[3] == '8') return "image/gif";
            if (data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F' &&
                data[8] == 'W' && data[9] == 'E' && data[10] == 'B' && data[11] == 'P') return "image/webp";
            return null;
        }
    }
}
