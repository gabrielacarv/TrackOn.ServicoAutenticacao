using Dapper;
using Microsoft.Data.SqlClient;
using TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces;
using TrackOn.ServicoAutenticacao.API.Entities.DTOs;
using Microsoft.Extensions.Configuration;

namespace TrackOn.ServicoAutenticacao.API.Infra.Repository
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly IConfiguration _configuracao;
        private readonly string _stringConexao;

        public RepositorioUsuario(IConfiguration configuracao)
        {
            _configuracao = configuracao;
            _stringConexao = _configuracao.GetConnectionString("DefaultConnection");
        }

        public async Task<UsuarioDTO> ObterUsuarioPorEmailESenha(string email, string senha)
        {
            using (var conexao = new SqlConnection(_stringConexao))
            {
                var consulta = "SELECT * FROM Clientes WHERE Email = @Email";
                var usuarioDTO = await conexao.QueryFirstOrDefaultAsync<UsuarioDTO>(consulta, new { Email = email });

                if (usuarioDTO != null && BCrypt.Net.BCrypt.Verify(senha, usuarioDTO.HashSenha))
                {
                    return usuarioDTO;
                }

                return null;
            }
        }

        public async Task CriarUsuario(UsuarioDTO usuario)
        {
            using (var conexao = new SqlConnection(_stringConexao))
            {
                var consulta = @"INSERT INTO Clientes 
                         (Email, HashSenha, Nome, CriadoEm, Ativo) 
                         VALUES (@Email, @HashSenha, @Nome, @CriadoEm, @Ativo)";

                await conexao.ExecuteAsync(consulta, new
                {
                    Email = usuario.Email,
                    HashSenha = usuario.HashSenha,
                    Nome = usuario.Nome,
                    CriadoEm = DateTime.Now,
                    Ativo = 1
                });
            }
        }

    }
}
