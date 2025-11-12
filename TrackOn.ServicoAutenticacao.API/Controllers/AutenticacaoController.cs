using Microsoft.AspNetCore.Mvc;
using TrackOn.ServicoAutenticacao.API.Entities.DTOs;
using TrackOn.ServicoAutenticacao.API.Services.Interfaces;

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
        public async Task<IActionResult> Login([FromBody] AutenticacaoDTO loginDTO)
        {
            var token = await _servicoAutenticacao.Autenticar(loginDTO.Email, loginDTO.Senha);

            if (token == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { Token = token });
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioDTO usuarioDTO)

        {
            try
            {
                await _servicoAutenticacao.CriarUsuario(usuarioDTO);
                return Ok("Usuário registrado com sucesso");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
