using Microsoft.AspNetCore.Mvc;
using TrackOn.ServicoAutenticacao.API.Entities.DTOs;
using TrackOn.ServicoAutenticacao.API.Services.Interfaces;
using TrackOn.ServicoAutenticacao.API.Services.Modelos;

namespace TrackOn.ServicoAutenticacao.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IServicoAutenticacao _servicoAutenticacao;

        public AutenticacaoController(IServicoAutenticacao servicoAutenticacao)
        {
            _servicoAutenticacao = servicoAutenticacao;
        }

        [HttpPost("autenticar")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] AutenticacaoRequest loginRequest)
        {
            var resultado = await _servicoAutenticacao.AutenticarAsync(loginRequest);

            if (!resultado.Sucesso)
            {
                var mensagemErro = resultado.Mensagem ?? "Não foi possível autenticar o usuário.";
                return resultado.MotivoFalha switch
                {
                    MotivoFalhaAutenticacao.CredenciaisInvalidas => Unauthorized(new ErrorResponse("credenciais_invalidas", mensagemErro)),
                    MotivoFalhaAutenticacao.DadosInvalidos => BadRequest(new ErrorResponse("dados_invalidos", mensagemErro)),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("erro_autenticacao", mensagemErro))
                };
            }

            return Ok(new TokenResponse(resultado.Token!));
        }

        [HttpPost("registrar")]
        [ProducesResponseType(typeof(UsuarioRegistradoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UsuarioRegistradoResponse>> Registrar([FromBody] RegistrarUsuarioRequest request)
        {
            var resultado = await _servicoAutenticacao.CriarUsuarioAsync(request);

            if (!resultado.Sucesso)
            {
                var mensagemErro = resultado.Mensagem ?? "Não foi possível registrar o usuário.";
                return resultado.MotivoFalha switch
                {
                    MotivoFalhaOperacao.DadosInvalidos => BadRequest(new ErrorResponse("dados_invalidos", mensagemErro)),
                    MotivoFalhaOperacao.Conflito => Conflict(new ErrorResponse("usuario_existente", mensagemErro)),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("erro_registro", mensagemErro))
                };
            }
            var response = new UsuarioRegistradoResponse(request.Email, request.Nome);
            return Created(string.Empty, response);
        }
    }
}
