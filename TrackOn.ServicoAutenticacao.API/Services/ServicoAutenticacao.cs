using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TrackOn.ServicoAutenticacao.API.Services.Interfaces;
using TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces;
using TrackOn.ServicoAutenticacao.API.Entities.DTOs;
using System.Security.Claims;
using System.Text;

namespace TrackOn.ServicoAutenticacao.API.Services
{
    public class ServicoAutenticacao : IServicoAutenticacao
    {
        private readonly IRepositorioUsuario _usuarioRepositorio;
        private readonly IConfiguration _configuracao;

        public ServicoAutenticacao(IRepositorioUsuario usuarioRepositorio, IConfiguration configuracao)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _configuracao = configuracao;
        }

        public async Task<string> Autenticar(string email, string senha)
        {
            var usuario = await _usuarioRepositorio.ObterUsuarioPorEmailESenha(email, senha);

            if (usuario == null)
            {
                return null;
            }

            var manipuladorToken = new JwtSecurityTokenHandler();
            var chave = Encoding.ASCII.GetBytes(_configuracao["Jwt:Key"]);
            var descricaoToken = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Email, usuario.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuracao["Jwt:ExpiryMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manipuladorToken.CreateToken(descricaoToken);
            return manipuladorToken.WriteToken(token);
        }

        public async Task CriarUsuario(UsuarioDTO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.HashSenha))
            {
                throw new ArgumentException("Email e senha são obrigatórios.");
            }

            usuario.HashSenha = BCrypt.Net.BCrypt.HashPassword(usuario.HashSenha);
            await _usuarioRepositorio.CriarUsuario(usuario);
        }
    }
}
