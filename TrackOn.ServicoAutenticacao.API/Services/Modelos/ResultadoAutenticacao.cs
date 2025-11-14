namespace TrackOn.ServicoAutenticacao.API.Services.Modelos
{
    public enum MotivoFalhaAutenticacao
    {
        Nenhum = 0,
        CredenciaisInvalidas,
        DadosInvalidos,
        ConfiguracaoInvalida
    }

    public class ResultadoAutenticacao
    {
        private ResultadoAutenticacao(bool sucesso, string? token, MotivoFalhaAutenticacao motivoFalha, string? mensagem)
        {
            Sucesso = sucesso;
            Token = token;
            MotivoFalha = motivoFalha;
            Mensagem = mensagem;
        }

        public bool Sucesso { get; }
        public string? Token { get; }
        public MotivoFalhaAutenticacao MotivoFalha { get; }
        public string? Mensagem { get; }

        public static ResultadoAutenticacao Falha(MotivoFalhaAutenticacao motivoFalha, string mensagem)
            => new(false, null, motivoFalha, mensagem);

        public static ResultadoAutenticacao CriarSucesso(string token)
            => new(true, token, MotivoFalhaAutenticacao.Nenhum, null);
    }
}