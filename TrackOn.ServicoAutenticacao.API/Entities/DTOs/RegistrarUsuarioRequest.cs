using System.ComponentModel.DataAnnotations;

namespace TrackOn.ServicoAutenticacao.API.Entities.DTOs
{
    public class RegistrarUsuarioRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;
    }
}