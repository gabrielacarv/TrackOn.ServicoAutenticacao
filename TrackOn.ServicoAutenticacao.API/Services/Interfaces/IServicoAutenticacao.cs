using TrackOn.ServicoAutenticacao.API.Entities.DTOs;

namespace TrackOn.ServicoAutenticacao.API.Services.Interfaces
{
    public interface IServicoAutenticacao
    {
        Task<string> Autenticar(string email, string senha);
        Task CriarUsuario(UsuarioDTO usuario);
    }
}
