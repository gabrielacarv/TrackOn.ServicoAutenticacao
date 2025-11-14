namespace TrackOn.ServicoAutenticacao.API.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string HashSenha { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
        public bool Ativo { get; set; }
    }
}
