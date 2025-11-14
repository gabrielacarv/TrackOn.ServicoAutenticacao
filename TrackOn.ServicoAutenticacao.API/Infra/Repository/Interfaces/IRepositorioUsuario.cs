using TrackOn.ServicoAutenticacao.API.Entities;
using TrackOn.ServicoAutenticacao.API.Entities.DTOs;

namespace TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces
{
    public interface IRepositorioUsuario
    {
        Task<Usuario?> ObterUsuarioPorEmailAsync(string email);
        Task CriarUsuarioAsync(Usuario usuario);
    }
}
