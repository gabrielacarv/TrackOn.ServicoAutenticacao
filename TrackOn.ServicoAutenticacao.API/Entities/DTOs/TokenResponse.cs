namespace TrackOn.ServicoAutenticacao.API.Entities.DTOs
{
    public class TokenResponse
    {
        public TokenResponse(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}