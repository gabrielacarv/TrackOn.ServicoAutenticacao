namespace TrackOn.ServicoAutenticacao.API.Services.Modelos
{
    public enum MotivoFalhaOperacao
    {
        Nenhum = 0,
        DadosInvalidos,
        Conflito,
        ErroIndeterminado
    }

    public class ResultadoOperacao
    {
        private ResultadoOperacao(bool sucesso, MotivoFalhaOperacao motivoFalha, string? mensagem)
        {
            Sucesso = sucesso;
            MotivoFalha = motivoFalha;
            Mensagem = mensagem;
        }

        public bool Sucesso { get; }
        public MotivoFalhaOperacao MotivoFalha { get; }
        public string? Mensagem { get; }

        public static ResultadoOperacao Falha(MotivoFalhaOperacao motivoFalha, string mensagem)
            => new(false, motivoFalha, mensagem);

        public static ResultadoOperacao CriarSucesso()
            => new(true, MotivoFalhaOperacao.Nenhum, null);
    }
}