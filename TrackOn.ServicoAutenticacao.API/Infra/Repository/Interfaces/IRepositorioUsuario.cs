using TrackOn.ServicoAutenticacao.API.Entities.DTOs;

namespace TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces
{
    public interface IRepositorioUsuario
    {
        Task<UsuarioDTO> ObterUsuarioPorEmailESenha(string email, string senha);
        Task CriarUsuario(UsuarioDTO usuario);
    }
}
