namespace TrackOn.ServicoAutenticacao.API.Entities.DTOs
{
    public class ErrorResponse
    {
        public ErrorResponse(string codigo, string mensagem)
        {
            Codigo = codigo;
            Mensagem = mensagem;
        }

        public string Codigo { get; }
        public string Mensagem { get; }
    }
}