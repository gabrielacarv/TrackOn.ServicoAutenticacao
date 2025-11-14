using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TrackOn.ServicoAutenticacao.API.Entities;
using TrackOn.ServicoAutenticacao.API.Entities.DTOs;
using TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces;
using TrackOn.ServicoAutenticacao.API.Services.Interfaces;
using TrackOn.ServicoAutenticacao.API.Services.Modelos;
using TrackOn.ServicoAutenticacao.API.Settings;
using System.Security.Claims;
using System.Text;

namespace TrackOn.ServicoAutenticacao.API.Services
{
    public class ServicoAutenticacao : IServicoAutenticacao
    {
        private readonly IRepositorioUsuario _usuarioRepositorio;
        private readonly JwtConfig _jwtSettings;

        public ServicoAutenticacao(IRepositorioUsuario usuarioRepositorio, IOptions<JwtConfig> jwtOptions)
        {
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _jwtSettings = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public async Task<ResultadoAutenticacao> AutenticarAsync(AutenticacaoRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return ResultadoAutenticacao.Falha(MotivoFalhaAutenticacao.DadosInvalidos, "Email e senha são obrigatórios.");
            }

            var usuario = await _usuarioRepositorio.ObterUsuarioPorEmailAsync(request.Email);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.HashSenha))
            {
                return ResultadoAutenticacao.Falha(MotivoFalhaAutenticacao.CredenciaisInvalidas, "Credenciais inválidas.");
            }

            var token = GerarToken(usuario.Email);
            return ResultadoAutenticacao.CriarSucesso(token);
        }

        public async Task<ResultadoOperacao> CriarUsuarioAsync(RegistrarUsuarioRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Senha) ||
                string.IsNullOrWhiteSpace(request.Nome))
            {
                return ResultadoOperacao.Falha(MotivoFalhaOperacao.DadosInvalidos, "Email, senha e nome são obrigatórios.");
            }

            var usuarioExistente = await _usuarioRepositorio.ObterUsuarioPorEmailAsync(request.Email);
            if (usuarioExistente != null)
            {
                return ResultadoOperacao.Falha(MotivoFalhaOperacao.Conflito, "Já existe um usuário cadastrado com o email informado.");
            }

            var usuario = new Usuario
            {
                Email = request.Email,
                Nome = request.Nome,
                HashSenha = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                CriadoEm = DateTime.UtcNow,
                Ativo = true
            };

            await _usuarioRepositorio.CriarUsuarioAsync(usuario);
            return ResultadoOperacao.CriarSucesso();
        }

        private string GerarToken(string email)
        {
            var handler = new JwtSecurityTokenHandler();
            var chave = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}