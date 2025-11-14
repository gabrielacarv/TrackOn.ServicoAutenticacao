using System.ComponentModel.DataAnnotations;

namespace TrackOn.ServicoAutenticacao.API.Settings
{
    public class JwtConfig
    {
        [Required]
        [MinLength(16)]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ExpiryMinutes { get; set; }

        public void EnsureValid()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}