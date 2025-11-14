using System.ComponentModel.DataAnnotations;

namespace TrackOn.ServicoAutenticacao.API.Config
{
    public class CorsConfig
    {
        [Required]
        public string PolicyName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

        public void EnsureValid()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}