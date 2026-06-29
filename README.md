# Backend Raízes do Nordeste – Guia de Configuração, Execução e Testes

---

## 1. Contextualização

Este projeto apresenta um MVP (Minimum Viable Product) da API Back-end da rede de lanchonetes **Raízes do Nordeste**. A solução foi desenvolvida para atender aos principais processos do negócio, incluindo autenticação via JWT, controle de acesso por perfis, operação multicanal (App, Web, Totem e Balcão), gerenciamento de pedidos e controle de estoque por unidade, auditoria de operações sensíveis, adequação à LGPD e integração simulada com um serviço externo de pagamento.

* **Trilha Escolhida:** Back-end
* **Status do MVP:** Funcional com persistência real em banco de dados e cobertura do **Fluxo [A]** (Pedido → Pagamento Mock → Atualização de Status).

---

## 2. Arquitetura

O projeto adota uma abordagem de **DDD Simplificado** (*Domain-Driven Design*) combinada com os princípios de separação de conceitos da **Clean Architecture**, utilizando um modelo de monoprojeto estruturado por pastas.


#### Responsabilidades de cada Camada

* **Domain (Camada de Domínio):** O núcleo da aplicação. Totalmente isolada de tecnologias externas ou dependências de frameworks.
  * **Entities:** Modelagem rica de dados do negócio (`Unidade`, `Pedido`, `Usuario`, `EstoqueMovimento`, `FidelidadeMovimento`, entre outras).
  * **Enums:** Tipagens estritas que governam os fluxos (`StatusPedido`, `CanalPedido`, `TipoMovimentoEstoque`).
  * **Interfaces:** Contratos abstratos dos repositórios e serviços utilitários centrais (como criptografia e tokens).

* **Application (Camada de Aplicação):** Orquestra o fluxo de dados da aplicação e mapeia os casos de uso.
  * **DTOs:** Objetos de transferência de dados customizados para requests e responses, fornecendo suporte flexível para fluxos complexos como compras anônimas via Totem (`ClienteId?`).
  * **Interfaces & Services:** Serviços de processamento de regras transacionais (`PedidoService`, `PagamentoService`, `EstoqueMovimentoService`) e Workers assíncronos em segundo plano (`CancelamentoPedidoWorker`).

* **Infrastructure (Camada de Infraestrutura):** Suporte tecnológico e operações de I/O externas.
  * **Data:** Configuração do ORM com o `ApplicationDbContext`.
  * **Data/Mappings:** Configurações via Fluent API (`PedidoMap`, `UsuarioMap`) traduzindo as restrições de negócio em esquemas relacionais no banco de dados.
  * **Data/Repositories:** Implementação concreta dos acessos a dados baseados nos contratos definidos no domínio.
  * **Security:** Mecanismos de infraestrutura utilitária, como hash de senhas (`PasswordHasher`) e geração de chaves `TokenService`.

* **Controllers (Camada de Apresentação):** Pontos de entrada da API REST.
  * Expõe os endpoints protegidos e públicos documentados via OpenAPI/Swagger, gerenciando o ciclo de vida das requisições HTTP e direcionando-as para os serviços da camada de aplicação correspondente.

---

## 3. Decisões de Projeto e Boas Práticas Adotadas

* **Controle transacional:** Operações críticas, como o processamento de pedidos e atualização de estoque, serão executadas de forma transacional, garantindo a consistência dos dados em caso de falhas durante a operação.

* **Suporte aos diferentes canais de atendimento:** A API foi projetada para atender aos canais App, Web, Totem e Balcão, permitindo tanto usuários autenticados quanto operações realizadas por terminais de autoatendimento.

* **Controle de acesso por perfis:** A autenticação será realizada por meio de tokens JWT e as funcionalidades da aplicação serão protegidas conforme o perfil do usuário (Cliente, Atendente, Cozinheiro, Gerente da Unidade e Administrador da Franquia).

* **Conformidade com a LGPD:** O sistema adota armazenamento seguro de senhas por meio de hash, registro do consentimento do usuário, auditoria das operações sensíveis e mecanismos de anonimização dos dados pessoais quando aplicável.

* **Documentação da API:** Todas as rotas da aplicação serão disponibilizadas por meio do Swagger/OpenAPI, facilitando os testes e a utilização da API durante o desenvolvimento e a avaliação do projeto.

---

## 4. Tecnologias Utilizadas e Pré-requisitos do Sistema

#### Tecnologias Utilizadas
* **Backend:** ASP.NET Core Web API (.NET 8 / C# 12)
* **Banco de Dados:** SQL Server
* **Persistência de Dados:** Entity Framework Core (ORM) / Migrations
* **Autenticação e Autorização:** JWT (JSON Web Tokens) e controle de acesso por perfis (Roles)
* **Documentação da API:** Swagger / OpenAPI
* **Ferramenta para Testes da API:** Postman

#### Pré-requisitos
Para executar a aplicação é necessário possuir:
* **.NET SDK:** versão 8.0 ou superior
* **SQL Server:** versão 2019 ou superior
* **Visual Studio:** versão 2022 ou superior (opcional)
* **Postman** versão 9.4.1
* **Git:** última versão estável


---

## 5. Guia de Configuração e Execução

#### 📁 Estrutura de Pastas do Repositório

```text
📁 raizes-do-nordeste-api
│
├── 📁 db/
│   └── 📄 database_seed.sql
│
├── 📁 docs/
│   ├── 📄 diagrama-der.pdf
│   ├── 📄 diagrama-classe.pdf
│   └── 📄 diagrama-sequencia.pdf
│
├── 📁 postman/
│   └── 📄 raizes-do-nordeste.postman_collection.json
│
├── 📁 src/
│   ├── 📄 RaizesDoNordeste.sln
│   └── 📁 RaizesDoNordeste.API/
│       ├── 📁 Properties/
│       ├── 📁 Application/
│       ├── 📁 Controllers/
│       ├── 📁 Domain/
│       ├── 📁 Infrastructure/
│       ├── 📁 Migrations/
│       ├── 📄 env.example
│       ├── 📄 appsettings.json
│       ├── 📄 Program.cs
│       └── 📄 RaizesDoNordeste.http
│
└── 📄 README.md
```


#### PASSO 1: Clonar o Repositório

1. Clone o repositório em sua máquina local utilizando o terminal:

```bash
git clone https://github.com/Josineteaz/raizes-do-nordeste-api.git
```

2. Abra o **Visual Studio 2022**.
3. Clique em **Arquivo (File)** → **Abrir (Open)** → **Projeto ou Solução (Project or Solution)**.
4. Navegue até a pasta do repositório clonado e selecione o arquivo `RaizesDoNordeste.sln`.
5. Clique em **Abrir**.

#### PASSO 2: Restaurar Dependências e Pacotes NuGet

Os pacotes necessários para execução da aplicação podem ser restaurados de duas formas:

* **Pela interface gráfica do Visual Studio:** com a solução carregada, clique com o botão direito sobre `Solution 'RaizesDoNordeste'` no Gerenciador de Soluções e selecione a opção **Restaurar Pacotes NuGet (Restore NuGet Packages)**.

* **Via linha de comando (CLI):** execute o comando abaixo na pasta raiz do projeto:

```bash
dotnet restore
```

#### PASSO 3: Configurar Variáveis de Ambiente

1. Acesse a pasta `RaizesDoNordeste.API`.
2. Localize o arquivo `env.example`.
> **Nota:** O arquivo foi disponibilizado sem o ponto inicial (`.env.example`) para facilitar seu versionamento e distribuição pelo GitHub e pelo ambiente Windows.
3. Renomeie o arquivo para `.env`.
4. Abra o arquivo e ajuste as variáveis conforme o seu ambiente local.

##### Configuração da conexão com o banco de dados

```bash
ConnectionStrings__DefaultConnection="Server=SeuServidor;Database=RaizesDoNordesteDb;User Id=SeuUsuario;Password=SuaSenha;TrustServerCertificate=True;"
```

⚠️ **Observações:**

* Substitua `SeuServidor` pelo nome da sua instância do SQL Server.
  Exemplos:

  * `localhost`
  * `(localdb)\MSSQLLocalDB`
  * `DESKTOP-XXXX\SQLEXPRESS`

* Substitua `SeuUsuario` pelo usuário com permissão de acesso ao banco.

* Substitua `SuaSenha` pela senha correspondente ao usuário informado.

✅ **Exemplo de configuração local:**

```bash
ConnectionStrings__DefaultConnection="Server=localhost;Database=RaizesDoNordesteDb;User Id=sa;Password=Senh@Segur@123;TrustServerCertificate=True;"
```

##### Configuração da chave JWT

A aplicação utiliza autenticação baseada em JSON Web Tokens (JWT). Defina uma chave secreta com, no mínimo, 32 caracteres:

```bash
JwtSettings__SecretKey="mudar_para_uma_chave_super_secreta_e_longa_com_mais_de_32_caracteres"
```

⚠️ **Importante:**

* A chave deve possuir pelo menos 32 caracteres.

✅ **Exemplo de configuração local:**

```bash
JwtSettings__SecretKey="8r7235uiqjfpwoqfrk09123$4163y5r2jrqórj091759821y5r98hrfoi%qwfjpaogfkapofjqpoityf$"
```
#### PASSO 4: Aplicar Migrations e Criar o Banco de Dados (EF Core)

Para criar o banco de dados e aplicar a estrutura de tabelas e relacionamentos utilizando o Entity Framework Core por meio do **Console do Gerenciador de Pacotes do Visual Studio**, siga os passos abaixo:

1. No **Gerenciador de Soluções**, clique com o botão direito em `RaizesDoNordeste.API` e selecione **Definir como Projeto de Inicialização** (*Set as Startup Project*).

2. No menu superior do Visual Studio, acesse:
   **Ferramentas** (*Tools*) → **Gerenciador de Pacotes NuGet** (*NuGet Package Manager*) → **Console do Gerenciador de Pacotes** (*Package Manager Console*).

3. Na barra superior do console, certifique-se de que o campo **Projeto padrão** (*Default Project*) esteja configurado para `RaizesDoNordeste.API`.

4. Execute o comando abaixo para aplicar as migrations existentes e criar o banco de dados, caso ele ainda não exista:

```powershell
Update-Database
```

Após a execução do comando, todas as tabelas, relacionamentos e restrições definidas pela aplicação estarão disponíveis no banco configurado no arquivo `.env`.

#### PASSO 5: Popular Dados Iniciais (Seed Data)

Para facilitar a execução dos testes e a utilização das rotas protegidas da API, é necessário carregar uma base inicial contendo usuários, perfis e demais dados operacionais do sistema.

1. Abra o **SQL Server Management Studio (SSMS)** e conecte-se à sua instância do SQL Server.
2. Expanda a pasta **Databases** e localize o banco de dados `RaizesDoNordesteDb`. Em seguida, clique com o botão direito sobre ele e selecione **New Query** (*Nova Consulta*).
3. Copie, cole e execute o conteúdo do script `database_seed.sql` localizado na pasta `db` do repositório.

> 🔐 **Nota de Segurança:**
>
> O script insere os usuários iniciais da aplicação com senhas previamente armazenadas em formato hash, seguindo boas práticas de segurança da informação e os princípios de proteção de dados estabelecidos pela LGPD.

#### PASSO 6: Iniciar a API

**Pelo Visual Studio:**

1. Certifique-se de que o projeto `RaizesDoNordeste.API` está definido como **Projeto de Inicialização** (*Startup Project*).

2. Pressione `F5` para executar a aplicação em **modo de depuração**, ou `Ctrl + F5` para executá-la **sem depuração**.

3. Aguarde a abertura da janela do terminal e a mensagem indicando que a aplicação está em execução.

> ✅ **Execução bem-sucedida:**  
> A API estará disponível nos endereços configurados no arquivo `launchSettings.json`, normalmente utilizando os protocolos HTTP e/ou HTTPS.

---

## 6. Guia de Testes

#### 6.1 Acessando a Documentação Swagger/OpenAPI

Com a aplicação em execução, a interface interativa do Swagger/OpenAPI pode ser acessada diretamente através do seu navegador

```bash
http://localhost:5081/swagger/index.html
```

A interface Swagger permite visualizar, documentar e executar as rotas da API diretamente pelo navegador.


#### 6.2 Testes via Postman

O projeto disponibiliza uma coleção Postman contendo exemplos de chamadas para os principais fluxos da aplicação. O arquivo encontra-se na pasta `postman` do projeto com o nome:

```text
raizes-do-nordeste.postman_collection.json
```
##### Importando a coleção

Para utilizar a coleção, siga os passos abaixo:

1. Abra o **Postman**.
2. Clique em **Import**.
3. Selecione o arquivo `raizes-do-nordeste.postman_collection.json` que se encontra na pasta `postman` do repositório.
4. Aguarde a importação da coleção.
5. Execute os testes na ordem que aparecem, observando suas pré-condições, quando existir.

##### Informações sobre a coleção

A coleção foi configurada para automatizar o processo de autenticação das rotas protegidas da API.

* **Captura automática do token JWT:**  
  A requisição de login possui um script de pós-execução (*Tests*) responsável por capturar o `accessToken` retornado pela API e armazená-lo automaticamente na variável `{{token_jwt}}`.

* **Herança do Bearer Token:**  
  As demais requisições protegidas utilizam a configuração de autorização do tipo **Bearer Token**, referenciando a variável `{{token_jwt}}`.

Dessa forma, após executar o login com sucesso, não é necessário copiar ou colar o token manualmente; todas as requisições subsequentes serão autenticadas automaticamente.


* **Pré-condições configuradas (encadeamento de testes / dependência de execução):**  
  Alguns testes fazem parte de fluxos encadeados e dependem da execução bem-sucedida de uma ou mais requisições anteriores para que o resultado esperado seja obtido.

  Essas dependências são previamente mapeadas e documentadas na aba `Pre-request Script` da requisição correspondente, indicando o fluxo necessário para sua execução.

  Caso a pré-condição não seja atendida, o teste poderá falhar devido à ausência de dados ou de contexto necessário para sua execução.

  Dessa forma, a execução da coleção deve respeitar a ordem definida no mapeamento de dependências.

##### Ordem de Execução dos Testes

| Código | Cenário | Objetivo |
| ------- | -------- | --------- |
| TE16 | Login - Senha inválida | Validar o retorno de erro para credenciais incorretas. |
| TE19 | Pedidos - Listar sem login | Validar o bloqueio de acesso a recursos protegidos sem autenticação. |
| TE17 | Cadastro - Tentativa de injeção de Perfil ADM | Validar a proteção contra elevação indevida de privilégios durante o cadastro. |
| T01 | Login do Cliente A | Validar a autenticação de um usuário do perfil Cliente A. |
| T06 | Criar Pedido para Unidade 2 | Validar a criação de um pedido para a Unidade 2. |
| T11 | Simular Pagamento | Validar o fluxo de processamento e confirmação do pagamento. |
| TE20 | Pedidos - Produto fora de época sazonal | Validar a restrição de venda de produtos indisponíveis por sazonalidade. |
| TE21 | Pedidos - Produto inexistente | Validar o tratamento para produtos não cadastrados. |
| TE22 | Pedidos - Falta de estoque na Unidade 1 | Validar a indisponibilidade de itens sem estoque. |
| TE23 | Pedidos - Quantidade negativa | Validar a rejeição de quantidades inválidas no pedido. |
| T02 | Login do Cliente B | Validar a autenticação de um usuário do perfil Cliente B. |
| T07 | Criar Pedido com Promoção e utilização de pontos de fidelidade | Validar a aplicação simultânea de promoções e resgate de pontos. |
| TE25 | Pagamentos - Pagamento recusado | Validar o tratamento de falhas no processamento do pagamento. |
| T13 | LGPD - Anonimização de usuário | Validar o processo de anonimização de dados pessoais conforme a LGPD. |
| T03 | Login do Cozinheiro da Unidade 2 | Validar a autenticação do perfil Cozinheiro da Unidade 2. |
| T08 | Mudar Status do Pedido para Em Preparo | Validar a transição do pedido para o status `EM_PREPARO`. |
| T09 | Mudar Status do Pedido para Pronto | Validar a transição do pedido para o status `PRONTO`. |
| T04 | Login do Atendente da Unidade 2 | Validar a autenticação do perfil Atendente da Unidade 2. |
| T10 | Atendente alterar Status do Pedido para Entregue | Validar a conclusão do pedido pelo perfil Atendente. |
| TE18 | Unidade - Tentativa de alteração com perfil sem permissão | Validar o controle de autorização por perfil de acesso. |
| TE24 | Pedidos - Transição proibida de status | Validar as regras de negócio para mudança de status do pedido. |
| T05 | Login do Administrador | Validar a autenticação do perfil Administrador. |
| T12 | Criar usuário com Perfil Atendente | Validar o cadastro de usuários do perfil Atendente. |
| T14 | Listar Usuários | Validar a consulta dos usuários cadastrados no sistema. |
| T15 | Consultar Logs | Validar a auditoria. |