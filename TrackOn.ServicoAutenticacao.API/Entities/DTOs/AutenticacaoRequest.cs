using System.ComponentModel.DataAnnotations;

namespace TrackOn.ServicoAutenticacao.API.Entities.DTOs
{
    public class AutenticacaoRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;
    }
}