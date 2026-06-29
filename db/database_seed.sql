-- ===============================================================================
-- DATABASE SEED - RAÍZES DO NORDESTE
-- ===============================================================================

USE [RaizesDoNordesteDb];
GO

-- 1. Limpar dados anteriores
DELETE FROM [Auditorias];
DELETE FROM [FidelidadeMovimentos];
DELETE FROM [Pagamentos];
DELETE FROM [PedidoItens];
DELETE FROM [Pedidos];
DELETE FROM [ProdutoSazonalidades]; 
DELETE FROM [ProdutoFichasTecnicas];
DELETE FROM [Promocoes];
DELETE FROM [Produtos]; 
DELETE FROM [Categorias];
DELETE FROM [EstoqueMovimentos];
DELETE FROM [Insumos]; 
DELETE FROM [Usuarios];
DELETE FROM [Unidades];

-- 2. Resetar os contadores IDENTITY para 0
DBCC CHECKIDENT ('[Auditorias]', RESEED, 0);
DBCC CHECKIDENT ('[FidelidadeMovimentos]', RESEED, 0);
DBCC CHECKIDENT ('[Pagamentos]', RESEED, 0);
DBCC CHECKIDENT ('[PedidoItens]', RESEED, 0);
DBCC CHECKIDENT ('[Pedidos]', RESEED, 0);
DBCC CHECKIDENT ('[ProdutoSazonalidades]', RESEED, 0);
DBCC CHECKIDENT ('[ProdutoFichasTecnicas]', RESEED, 0);
DBCC CHECKIDENT ('[Promocoes]', RESEED, 0);
DBCC CHECKIDENT ('[Produtos]', RESEED, 0);
DBCC CHECKIDENT ('[Categorias]', RESEED, 0);
DBCC CHECKIDENT ('[EstoqueMovimentos]', RESEED, 0);
DBCC CHECKIDENT ('[Insumos]', RESEED, 0);
DBCC CHECKIDENT ('[Usuarios]', RESEED, 0);
DBCC CHECKIDENT ('[Unidades]', RESEED, 0);

-- -------------------------------------------------------------------------------
-- 1. TABELA: Unidades
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [Unidades] ON;

INSERT INTO [Unidades] ([Id], [Cnpj], [Nome], [Telefone], [Cep], [Logradouro], [Numero], [Bairro], [Cidade], [Estado], [TipoCozinha], [HorarioAbertura], [HorarioFechamento], [TaxaEntrega], [Ativo]) VALUES 
(1, '12345678000199', 'Raízes do Nordeste - Unidade Recife', '8134445555', '50012000', 'Avenida Agamenon Magalhães', '1500', 'Boa Vista', 'Recife', 'PE', 1, '11:00:00', '23:00:00', 7.50, 1),
(2, '98765432000288', 'Raízes do Nordeste - Unidade Olinda', '8134446666', '53010000', 'Rua do Amparo', '45', 'Sítio Histórico', 'Olinda', 'PE', 2, '16:00:00', '00:00:00', 9.00, 1);

SET IDENTITY_INSERT [Unidades] OFF;
PRINT '-> Unidades populadas.';

-- -------------------------------------------------------------------------------
-- 2. TABELA: Categorias
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [Categorias] ON;

INSERT INTO [Categorias] ([Id], [UnidadeId], [Nome], [Ativo]) VALUES 
(1, 1, 'Bebidas Regionais', 1),
(2, 1, 'Comidas Típicas', 1),
(3, 2, 'Bebidas Regionais', 1),
(4, 2, 'Comidas Típicas', 1);

SET IDENTITY_INSERT [Categorias] OFF;
PRINT '-> Categorias populadas.';

-- -------------------------------------------------------------------------------
-- 3. TABELA: Insumos
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [Insumos] ON;

INSERT INTO [Insumos] ([Id], [UnidadeId], [Nome], [UnidadeMedida], [QuantidadeMinima], [QuantidadeAtual], [Ativo]) VALUES 
-- Insumos da Matriz (Unidade 1 - Recife)
(1, 1, 'Polpa de Umbu-Cajá', 'KG', 5.0000, 25.0000, 1),
(2, 1, 'Goma de Mandioca Hidratada', 'KG', 5.0000, 10.0000, 1),
(3, 1, 'Queijo Coalho Artesanal', 'KG', 8.0000, 30.0000, 1),
(4, 1, 'Macaxeira In Natura Ralada', 'KG', 15.0000, 50.0000, 1),
-- Insumos da Filial (Unidade 2 - Olinda)
(5, 2, 'Goma de Mandioca Hidratada', 'KG', 5.0000, 20.0000, 1),
(6, 2, 'Queijo Coalho Artesanal', 'KG', 4.0000, 15.0000, 1),
(7, 2, 'Macaxeira In Natura Ralada', 'KG', 18.0000, 35.0000, 1); 

SET IDENTITY_INSERT [Insumos] OFF;
PRINT '-> Insumos populados.';

-- -------------------------------------------------------------------------------
-- 4. TABELA: Usuarios
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [Usuarios] ON;

INSERT INTO [Usuarios] ([Id], [Nome], [Email], [SenhaHash], [DataInclusao], [Perfil], [UnidadeId], [Cpf], [DataNascimento], [Telefone], [Cep], [Logradouro], [Numero], [Bairro], [Cidade], [Estado], [ConsentimentoLgpd], [DataConsentimento], [SaldoPontos], [Ativo]) VALUES 
-- Administrador da Franquia (Perfil: 5)
(1, 'Antônio Neto - Admin Franquia', 'admin.neto@raizesdonordeste.com', '$2a$11$TflO0ic3.OC1b4SnQMQ9bOC8kRLZxYLRTgaj.af/3gh05znUvw74O', GETUTCDATE(), 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, GETUTCDATE(), NULL, 1),
-- Gerente Unidade 1 (Perfil: 4)
(2, 'Dona Francisca Gestora', 'francisca@raizesdonordeste.com', '$2a$11$TflO0ic3.OC1b4SnQMQ9bOC8kRLZxYLRTgaj.af/3gh05znUvw74O', GETUTCDATE(), 4, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, GETUTCDATE(), NULL, 1),
-- Gerente Unidade 2 (Perfil: 4)
(3, 'Ricardo Almeida - Gerente U2', 'ricardo.gerencia2@raizesdonordeste.com', '$2a$11$TflO0ic3.OC1b4SnQMQ9bOC8kRLZxYLRTgaj.af/3gh05znUvw74O', GETUTCDATE(), 4, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, GETUTCDATE(), NULL, 1),
-- Atendente Unidade 1 (Perfil: 2)
--(4, 'Severino dos Santos', 'severino.atendimento@raizesdonordeste.com', '$2a$11$TflO0ic3.OC1b4SnQMQ9bOC8kRLZxYLRTgaj.af/3gh05znUvw74O', GETUTCDATE(), 2, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, GETUTCDATE(), NULL, 1),
-- Cozinheiro Unidade 1 (Perfil: 3)
(5, 'Maria das Dores (Maria Cozinha)', 'maria.cozinha@raizesdonordeste.com', '$2a$11$TflO0ic3.OC1b4SnQMQ9bOC8kRLZxYLRTgaj.af/3gh05znUvw74O', GETUTCDATE(), 3, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, GETUTCDATE(), NULL, 1),
-- Atendente Unidade 2 (Perfil: 2)
(6, 'José da Silva - Atendente U2', 'jose.atendimento2@raizesdonordeste.com', '$2a$11$TflO0ic3.OC1b4SnQMQ9bOC8kRLZxYLRTgaj.af/3gh05znUvw74O', GETUTCDATE(), 2, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, GETUTCDATE(), NULL, 1),
-- Cozinheiro Unidade 2 (Perfil: 3)
(7, 'Luzia de Souza - Cozinha U2', 'luzia.cozinha2@raizesdonordeste.com', '$2a$11$TflO0ic3.OC1b4SnQMQ9bOC8kRLZxYLRTgaj.af/3gh05znUvw74O', GETUTCDATE(), 3, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, GETUTCDATE(), NULL, 1),
-- Cliente (Perfil: 1)
(8, 'João Silva', 'joao.silva@email.com', '$2a$11$Eeg0TmW2ATyXmZ0.9fvaxObHEGJIw/LPiLDvCRF.VuzPnGoD2Vqgy', GETUTCDATE(), 1, NULL, '99988877766', '1995-10-20', '81977776666', '51000000', 'Rua dos Navegantes', '500', 'Boa Viagem', 'Recife', 'PE', 1, GETUTCDATE(), 50, 1),
-- Cliente (Perfil: 1)
(9, 'Luzia Sousa', 'luzia.sousa@email.com', '$2a$11$Eeg0TmW2ATyXmZ0.9fvaxObHEGJIw/LPiLDvCRF.VuzPnGoD2Vqgy', GETUTCDATE(), 1, NULL, '99944477766', '1990-08-03', '81977721666', '51000000', 'Rua dos Navegantes', '501', 'Boa Viagem', 'Recife', 'PE', 1, GETUTCDATE(), 50, 1);

SET IDENTITY_INSERT [Usuarios] OFF;
PRINT '-> Usuários de teste configurados.';

-- -------------------------------------------------------------------------------
-- 5. TABELA: Produtos
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [Produtos] ON;

INSERT INTO [Produtos] ([Id], [UnidadeId], [CategoriaId], [Nome], [Preco], [UrlImagem], [Ativo]) VALUES 
-- Produtos Unidade 1 (Matriz)
(1, 1, 1, 'Suco de Umbu-Cajá Litro', 12.00, 'https://cdn.raizesdonordeste.com/imagens/suco-umbu-caja.jpg', 1), 
(2, 1, 2, 'Tapioca Rendada com Queijo Coalho', 14.50, 'https://cdn.raizesdonordeste.com/imagens/tapioca-coalho.jpg', 1),
(3, 1, 2, 'Fatia de Bolo de Macaxeira Tradicional com Doce de Leite', 11.90, 'https://cdn.raizesdonordeste.com/imagens/bolo-macaxeira-gourmet.jpg', 1), -- Ajustado para Passo 14 (Gourmet)
-- Produtos Unidade 2 (Filial)
(4, 2, 4, 'Tapioca Rendada com Queijo Coalho', 16.00, 'https://cdn.raizesdonordeste.com/imagens/tapioca-coalho.jpg', 1), -- Mapeado de acordo com IDs de cat da U2
(5, 2, 4, 'Fatia de Bolo de Macaxeira Tradicional', 9.50, 'https://cdn.raizesdonordeste.com/imagens/bolo-macaxeira.jpg', 1);

SET IDENTITY_INSERT [Produtos] OFF;
PRINT '-> Produtos populados.';

-- -------------------------------------------------------------------------------
-- 6. TABELA: ProdutoSazonalidades
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [ProdutoSazonalidades] ON;

-- Mapeamento do array do Produto 1: Meses Julho (7) e Agosto (8)
INSERT INTO [ProdutoSazonalidades] ([Id], [ProdutoId], [Mes], [Descricao]) VALUES 
(1, 1, 4, 'Disponível exclusivamente durante o período de alta estação em Abril e Maio.'),
(2, 1, 5, 'Disponível exclusivamente durante o período de alta estação em Abril e Maio.');

SET IDENTITY_INSERT [ProdutoSazonalidades] OFF;
PRINT '-> Regras de Sazonalidade vinculadas.';

-- -------------------------------------------------------------------------------
-- 7. TABELA: ProdutoFichasTecnicas
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [ProdutoFichasTecnicas] ON;

INSERT INTO [ProdutoFichasTecnicas] ([Id], [ProdutoId], [InsumoId], [Quantidade], [AparecerMenu]) VALUES 
-- Vinculações da Unidade 1
(1, 1, 1, 0.2000, 1), -- Suco U1 -> Polpa (200g)
(2, 2, 2, 0.1800, 1), -- Tapioca U1 -> Goma (180g)
(3, 2, 3, 0.0800, 1), -- Tapioca U1 -> Queijo Coalho (80g)
(4, 3, 4, 0.2500, 1), -- Bolo U1 -> Macaxeira Ralada (250g)
-- Vinculações da Unidade 2
(5, 4, 5, 0.1500, 1), -- Tapioca U2 -> Goma U2 (150g)
(6, 4, 6, 0.0800, 1), -- Tapioca U2 -> Queijo U2 (80g)
(7, 5, 7, 0.2500, 1); -- Bolo U2 -> Macaxeira U2 (250g)

SET IDENTITY_INSERT [ProdutoFichasTecnicas] OFF;
PRINT '-> Fichas Técnicas integradas.';

-- -------------------------------------------------------------------------------
-- 8. TABELA: Promocoes
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [Promocoes] ON;

INSERT INTO [Promocoes] ([Id], [UnidadeId], [ProdutoId], [Nome], [Descricao], [TipoDesconto], [ValorDesconto], [DataInicio], [DataFim]) VALUES 
(1, 2, 4, 'Festival de Inverno - Tapioca Quentinha', 'Para aquecer as noites de inverno: nossa tapioca rendada com 15% de desconto!', 1, 15.00, '2026-06-01 00:00:00', '2026-10-31 23:59:59');

SET IDENTITY_INSERT [Promocoes] OFF;
PRINT '-> Campanhas Promocionais carregadas.';

-- -------------------------------------------------------------------------------
-- 9. TABELA: EstoqueMovimentos
-- -------------------------------------------------------------------------------
SET IDENTITY_INSERT [EstoqueMovimentos] ON;

INSERT INTO [EstoqueMovimentos] ([Id], [UnidadeId], [InsumoId], [TipoMovimento], [Quantidade], [DataMovimento], [Observacao]) VALUES 
-- Movimentações Iniciais Matriz (Unidade 1)
(1, 1, 1, 1, 25.0000, GETUTCDATE(), 'Carga inicial de polpas para o Verão'),
(2, 1, 2, 1, 40.0000, GETUTCDATE(), 'Abastecimento semanal de goma'),
(3, 1, 3, 1, 30.0000, GETUTCDATE(), 'Recebimento de queijo artesanal direto do produtor'),
(4, 1, 4, 1, 50.0000, GETUTCDATE(), 'Abastecimento de macaxeira para os bolos'),
-- Movimentações Iniciais Filial (Unidade 2)
(5, 2, 5, 1, 20.0000, GETUTCDATE(), 'Abastecimento inicial filial João Pessoa'),
(6, 2, 6, 1, 15.0000, GETUTCDATE(), 'Entrega de queijo coalho U2'),
(7, 2, 7, 1, 35.0000, GETUTCDATE(), 'Carga de macaxeira para produção de bolos U2');

SET IDENTITY_INSERT [EstoqueMovimentos] OFF;
PRINT '-> Histórico de Movimentação do Estoque carregado.';


PRINT '===============================================================================';
PRINT '  TODOS OS DADOS FORAM INSERIDOS COM SUCESSO NO BANCO RAÍZES DO NORDESTE!  ';
PRINT '===============================================================================';
GO