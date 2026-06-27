namespace RaizesDoNordeste.API.Application.DTOs
{
    public class FidelidadeResultadoDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int NovoSaldo { get; set; }

        public static FidelidadeResultadoDto SucessoResultado(int novoSaldo, string mensagem = "Resgate processado!")
        {
            return new FidelidadeResultadoDto { Sucesso = true, NovoSaldo = novoSaldo, Mensagem = mensagem };
        }

        public static FidelidadeResultadoDto Falha(string mensagem)
        {
            return new FidelidadeResultadoDto { Sucesso = false, Mensagem = mensagem };
        }
    }
}