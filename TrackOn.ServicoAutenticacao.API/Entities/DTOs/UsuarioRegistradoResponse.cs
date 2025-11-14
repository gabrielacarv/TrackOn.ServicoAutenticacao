namespace TrackOn.ServicoAutenticacao.API.Entities.DTOs
{
    public class UsuarioRegistradoResponse
    {
        public UsuarioRegistradoResponse(string email, string nome)
        {
            Email = email;
            Nome = nome;
        }

        public string Email { get; }
        public string Nome { get; }
    }
}