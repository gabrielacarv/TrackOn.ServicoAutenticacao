using TrackOn.ServicoAutenticacao.API.Entities.DTOs;
using TrackOn.ServicoAutenticacao.API.Services.Modelos;

namespace TrackOn.ServicoAutenticacao.API.Services.Interfaces
{
    public interface IServicoAutenticacao
    {
        Task<ResultadoAutenticacao> AutenticarAsync(AutenticacaoRequest request);
        Task<ResultadoOperacao> CriarUsuarioAsync(RegistrarUsuarioRequest request);
    }
}
