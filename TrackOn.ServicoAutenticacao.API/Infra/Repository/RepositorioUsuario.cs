using Dapper;
using Microsoft.Data.SqlClient;
using TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces;
using TrackOn.ServicoAutenticacao.API.Entities;

namespace TrackOn.ServicoAutenticacao.API.Infra.Repository
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private const string ConsultaUsuarioPorEmail = @"SELECT Id, Email, HashSenha, Nome, CriadoEm, Ativo
                                                         FROM Clientes
                                                         WHERE Email = @Email";

        private const string ComandoInserirUsuario = @"INSERT INTO Clientes (Email, HashSenha, Nome, CriadoEm, Ativo)
                                                       VALUES (@Email, @HashSenha, @Nome, @CriadoEm, @Ativo)";

        private readonly string _stringConexao;

        public RepositorioUsuario(IConfiguration configuracao)
        {
            ArgumentNullException.ThrowIfNull(configuracao);
            _stringConexao = configuracao.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não foi configurada.");
        }

        public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            await using var conexao = new SqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<Usuario>(ConsultaUsuarioPorEmail, new { Email = email });
        }

        public async Task CriarUsuarioAsync(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            await using var conexao = new SqlConnection(_stringConexao);
            await conexao.ExecuteAsync(ComandoInserirUsuario, new
            {
                usuario.Email,
                usuario.HashSenha,
                usuario.Nome,
                usuario.CriadoEm,
                Ativo = usuario.Ativo ? 1 : 0
            });
        }

    }
}
