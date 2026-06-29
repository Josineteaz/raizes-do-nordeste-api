using RaizesDoNordeste.API.Application.DTOs;
using RaizesDoNordeste.API.Application.Interfaces;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Enums;
using RaizesDoNordeste.API.Domain.Interfaces;


namespace RaizesDoNordeste.API.Application.Services
{
    public class FidelidadeService : IFidelidadeService
    {
        private readonly IFidelidadeRepository _fidelidadeRepository;
        private readonly IUsuarioRepository _usuarioRepository;


        public FidelidadeService(IUsuarioRepository usuarioRepository, IFidelidadeRepository fidelidadeRepository)
        {
            _fidelidadeRepository = fidelidadeRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<int> ObterPontosPorClienteAsync(int clienteId)
        {
            var cliente = await _usuarioRepository.ObterPorIdAsync(clienteId);
            if (cliente == null)
            {
                return 0;
            }

            int saldoTotal = cliente.SaldoPontos ?? 0;

            return saldoTotal < 0 ? 0 : saldoTotal;

        }

        public async Task<FidelidadeResultadoDto> ProcessarResgateAsync(ResgatePontosDto resgateDto)
        {
            if (resgateDto.PontosParaResgatar < 50 || resgateDto.PontosParaResgatar % 50 != 0)
            {
                return FidelidadeResultadoDto.Falha("O resgate mínimo é de 50 pontos e deve ser realizado em múltiplos de 50 (ex: 50, 100, 150...).");
            }

            int saldoAtual = await ObterPontosPorClienteAsync(resgateDto.ClienteId);

            if (saldoAtual < resgateDto.PontosParaResgatar)
            {
                return FidelidadeResultadoDto.Falha($"Saldo de pontos insuficiente. Você possui {saldoAtual} pontos e tentou resgatar {resgateDto.PontosParaResgatar}.");
            }

            var debitoPontos = new FidelidadeMovimento
            {
                ClienteId = resgateDto.ClienteId,
                TipoMovimento = TipoMovimentoFidelidade.Resgate,
                Pontos = resgateDto.PontosParaResgatar
            };

            await _fidelidadeRepository.AdicionarMovimentacaoAsync(debitoPontos);

            
            decimal valorDescontoGerado = (resgateDto.PontosParaResgatar / 50) * 5;
            int novoSaldo = saldoAtual - resgateDto.PontosParaResgatar;

            
            string mensagemSucesso = $"Pontos resgatados com sucesso! Você gerou um desconto de R$ {valorDescontoGerado:F2}.";

            return FidelidadeResultadoDto.SucessoResultado(novoSaldo, mensagemSucesso);
        }

        public async Task<IEnumerable<FidelidadeMovimento>> ObterMovimentacoesPorClienteAsync(int clienteId, int page, int limit)
        {
            var cliente = await _usuarioRepository.ObterPorIdAsync(clienteId);
            if (cliente == null)
            {
                return Enumerable.Empty<FidelidadeMovimento>();
            }

            return await _fidelidadeRepository.ObterHistoricoPorClientePaginadoAsync(clienteId, page, limit);
        }
    }
}