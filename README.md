# TechsysLog

Sistema para **gestão de pedidos e entregas** com notificações em tempo real, autenticação JWT e dashboard reativo.

Stack: **.NET 9 (Clean Architecture + CQRS)** no backend, **Angular 20 (standalone + Signals)** no frontend, **MongoDB 7** como banco e **SignalR** para realtime.

## Sumário

1. [Executar via Docker](#1-executar-via-docker)
2. [Executar em localhost (sem Docker)](#2-executar-em-localhost-sem-docker)
3. [Estrutura de pastas](#3-estrutura-de-pastas)
   - [3.1. Backend](#31-backend)
   - [3.2. Frontend](#32-frontend)
4. [Arquitetura e camada de segurança JWT](#4-arquitetura-e-camada-de-segurança-jwt)
5. [Dashboard realtime](#5-dashboard-realtime)
6. [Fluxo de dados e sugestões de testes](#6-fluxo-de-dados-e-sugestões-de-testes)

---

## 1. Executar via Docker

**Pré-requisito:** Docker Engine + Docker Compose v2.

Na raiz do projeto:

```bash
docker compose up -d --build
```

Aguarde o `mongo` ficar healthy (~10s) — `backend` e `frontend` sobem em sequência.

| Serviço         | URL                                  | Observação                          |
|-----------------|--------------------------------------|-------------------------------------|
| Frontend        | `http://localhost:4200`              | App Angular                         |
| Backend (API)   | `http://localhost:5077`              | ASP.NET Core 9                      |
| Swagger         | `http://localhost:5077/swagger`      | Documentação interativa             |
| MongoDB         | `mongodb://localhost:27017`          | Database `techsyslog`               |
| Mongo Express   | `http://localhost:8081`              | UI do Mongo (admin / admin)         |

Para parar e remover tudo (incluindo volume do banco):

```bash
docker compose down -v
```

---

## 2. Executar em localhost (sem Docker)

**Pré-requisitos:**
- .NET 9 SDK
- Node.js 20+ e npm
- MongoDB 7 rodando localmente OU `docker compose up -d mongo mongo-express`

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/TechsysLog.API
```

API disponível em `http://localhost:5077` (porta padrão de `appsettings.json`).
Swagger: `http://localhost:5077/swagger`.

Configurações relevantes em `backend/src/TechsysLog.API/appsettings.json`:

```json
{
  "Mongo":    { "ConnectionString": "mongodb://localhost:27017", "Database": "techsyslog" },
  "Jwt":      { "Issuer": "TechsysLog", "Audience": "TechsysLog", "Key": "<secret>", "AccessTokenMinutes": 15, "RefreshTokenDays": 7 },
  "Cep":      { "BaseUrl": "https://viacep.com.br/ws/" },
  "Password": { "Pepper": "<base64 com 64 bytes>" }
}
```

> **Importante:** `Password.Pepper` deve ser **exatamente 64 bytes** decodificados de base64. Caso contrário a API falha ao iniciar com mensagem `Password.Pepper deve ter exatamente 64 bytes...`. Para gerar uma chave nova:
> ```powershell
> $rng=[System.Security.Cryptography.RNGCryptoServiceProvider]::new(); $b=New-Object byte[] 64; $rng.GetBytes($b); [Convert]::ToBase64String($b)
> ```

### Frontend

```bash
cd frontend
npm install
npm start
```

App: `http://localhost:4200`. Conecta automaticamente em `http://localhost:5077` (configurado em `src/environments/environment.ts`).

---

## 3. Estrutura de pastas

```
TechsysLog/
├── backend/                      # .NET 9 (Clean Architecture + CQRS)
├── frontend/                     # Angular 20 (standalone + Signals)
├── docker-compose.yml            # Mongo + Mongo Express + backend + frontend
└── README.md
```

### 3.1. Backend

```
backend/
├── TechsysLog.sln
├── Dockerfile
└── src/
    ├── TechsysLog.Domain/                Entidades, enums, exceções de domínio
    │   ├── Common/                       (BaseEntity, value objects)
    │   ├── Entities/                     (Order, User, Delivery, Notification, RefreshToken, Address)
    │   ├── Enums/                        (OrderStatus, NotificationType)
    │   └── Exceptions/                   (DomainException, NotFoundException, ConflictException, UnauthorizedException)
    │
    ├── TechsysLog.Application/           Casos de uso (CQRS via MediatR)
    │   ├── Common/
    │   │   ├── Behaviors/                (ValidationBehavior — pipeline FluentValidation)
    │   │   ├── Errors/                   (ValidationProblem)
    │   │   ├── Exceptions/               (ValidationException da app)
    │   │   ├── Interfaces/               (IOrderRepository, IJwtTokenService, ICepService, ...)
    │   │   └── Security/                 (TokenHasher)
    │   ├── Features/
    │   │   ├── Auth/                     (Register, Login, Refresh, Logout)
    │   │   ├── Orders/                   (CreateOrder, GetOrders, UpdateStatus, GetOrderDetails)
    │   │   ├── Deliveries/               (CreateDelivery)
    │   │   ├── Notifications/            (GetNotifications, MarkAsRead)
    │   │   ├── Cep/                      (LookupCep)
    │   │   └── Dashboard/                (GetDashboard)
    │   └── DependencyInjection.cs
    │
    ├── TechsysLog.Infrastructure/        Adaptadores externos (Mongo, JWT, ViaCEP, SignalR)
    │   ├── Auth/                         (JwtTokenService, JwtOptions, PasswordHasher, PasswordOptions)
    │   ├── External/                     (ViaCepService, CepOptions)
    │   ├── Persistence/                  (MongoContext, MongoConventions, repositories)
    │   ├── Realtime/                     (SignalRRealtimeNotifier, NotificationHub)
    │   └── DependencyInjection.cs
    │
    └── TechsysLog.API/                   Composição final (controllers + middleware)
        ├── Controllers/                  (Auth, Orders, Deliveries, Notifications, Cep, Dashboard)
        ├── Middleware/                   (ExceptionHandlingMiddleware)
        ├── Services/                     (CurrentUserService)
        ├── appsettings.json
        ├── appsettings.Development.json
        └── Program.cs
```

### 3.2. Frontend

```
frontend/
├── Dockerfile
├── nginx.conf
├── package.json
├── angular.json
└── src/
    ├── environments/                     (environment.ts, environment.prod.ts)
    ├── styles.scss
    ├── index.html
    ├── main.ts
    └── app/
        ├── app.config.ts                 (providers: HttpClient + interceptors + Router)
        ├── app.routes.ts                 (rotas + authGuard)
        ├── core/
        │   ├── guards/                   (authGuard)
        │   ├── interceptors/             (jwtInterceptor, errorInterceptor com retry+fila)
        │   ├── models/                   (DTOs espelhados do backend)
        │   └── services/                 (AuthService, OrderService, DeliveryService,
        │                                  NotificationService, RealtimeService, CepService,
        │                                  DashboardService, TokenRefreshCoordinatorService)
        ├── features/
        │   ├── auth/                     (login, register)
        │   ├── dashboard/                (cards + grid + modal de detalhes)
        │   ├── orders/                   (lista + criação + modal mudar status)
        │   ├── deliveries/               (registrar entrega)
        │   └── notifications/            (lista de notificações)
        └── shared/
            └── layout/                   (ShellComponent — sidebar + topbar com sino)
```

---

## 4. Arquitetura e camada de segurança JWT

### Clean Architecture + CQRS

O backend segue **Clean Architecture** com 4 projetos:

- **Domain** — entidades puras + invariantes (ex.: `Order.ChangeStatus` com state machine).
- **Application** — casos de uso via **CQRS com MediatR** (Commands para mutações, Queries para leituras), validados por **FluentValidation** num `IPipelineBehavior` global.
- **Infrastructure** — implementações concretas (MongoDB, JWT, ViaCEP, SignalR Hub).
- **API** — controllers finos que apenas despacham `IMediator.Send(...)`.

Erros de domínio (`DomainException`, `NotFoundException`, `ConflictException`, `UnauthorizedException`) são traduzidos para HTTP pelo `ExceptionHandlingMiddleware`.

### Autenticação JWT + Refresh com rotação

| Aspecto | Implementação |
|---|---|
| Hash de senha | **BCrypt + pepper HMAC-SHA512** (chave de 64 bytes em `appsettings.Password.Pepper`). Hashes têm prefixo `p1$`. |
| Login | `POST /auth/login` retorna `{ accessToken, refreshToken, expiresIn }`. Refresh é persistido em `refresh_tokens` (Mongo) com hash SHA-256, `FamilyId` e `ExpiresAt`. |
| Access token | JWT HS256 com TTL de 15 minutos. Claims: `sub`, `nameid`, `name`, `email`, `jti`. |
| Refresh | `POST /auth/refresh` com `{ refreshToken }` → novo par + revoga o antigo (rotação obrigatória). Reuso de token revogado revoga **toda a família** (defesa contra roubo). |
| Logout | `POST /auth/logout` revoga o refresh server-side (idempotente). |
| Frontend | `errorInterceptor` em 401 usa `TokenRefreshCoordinatorService` (singleton com fila) para tentar refresh **uma vez** antes de deslogar — coalesce múltiplos 401 paralelos em **um único** refresh. |
| Autorização | Apenas `[Authorize]` global (sem roles); todo usuário autenticado tem acesso aos endpoints. |

### Modelo de usuário

- `User { Id, Name, Email, PasswordHash }` — sem campo `Role` (removido por decisão de produto).
- Re-hash transparente: usuários cuja senha foi cadastrada antes do pepper são re-hasheados automaticamente no próximo login bem-sucedido.

---

## 5. Dashboard realtime

O `/dashboard` exibe (filtrado por mês/ano):

- **5 cards clicáveis** com contadores por status (`Criado`, `Processando`, `Enviado`, `Entregue`, `Cancelado`).
- **Uma grid única** abaixo, sincronizada com o card selecionado (e com `<select>` de status — opção `Todos` por default).
- **Botão `Detalhes`** em cada linha abre modal com dados completos do `Order` + lista de `Deliveries` (via `GET /orders/{id}/details`).

### Como o realtime funciona

1. **Backend**: handlers de mutação (`CreateOrderCommandHandler`, `UpdateOrderStatusCommandHandler`, `CreateDeliveryCommandHandler`) chamam `IRealtimeNotifier.PublishAsync(eventName, payload)` ao final do fluxo.
2. **SignalR Hub** (`/hubs/notifications`) faz broadcast para todos os clientes conectados.
3. **Frontend** (`RealtimeService`) ouve cada evento (`OrderCreated`, `OrderStatusChanged`, `DeliveryRegistered`, `Notification`) e emite no `events$: Subject`.
4. **Componentes** (Dashboard, Orders, Deliveries) inscrevem-se em `events$` filtrando os eventos relevantes; quando um chega, refazem a chamada HTTP que alimenta a tela.
5. O **sino no topbar** (`ShellComponent`) mostra contador de notificações não lidas via signal `NotificationService.unreadCount()`, atualizado pelo mesmo canal.

Conexão SignalR é estabelecida no `ShellComponent.ngOnInit` com o JWT atual. Em desconexões, `withAutomaticReconnect()` tenta restabelecer.

---

## 6. Fluxo de dados e sugestões de testes

### Fluxo principal — criar pedido + entregar

```
[Cliente] POST /auth/login
   └─> JWT + RefreshToken persistido
[Cliente] POST /orders { orderNumber, value, cep, number, complement?, observation? }
   ├─> Valida orderNumber (regex ^\d+$, max 20) e unicidade (409 se duplicado)
   ├─> ViaCEP enriquece endereço
   ├─> Order persistido (status=Created)
   ├─> Notification salva
   └─> SignalR broadcast: "OrderCreated"
[Operador] PUT /orders/{id}/status { "status": "Processing" }
   └─> Order.ChangeStatus(state machine) + SignalR "OrderStatusChanged"
[Operador] PUT /orders/{id}/status { "status": "Shipped" }
[Operador] POST /deliveries { orderId, deliveredAt, notes? }
   ├─> Delivery persistido
   ├─> Order.MarkAsDelivered() (força Delivered, exceto se Cancelled)
   └─> SignalR "DeliveryRegistered" + "OrderStatusChanged"
```

### Fluxo de notificação

Toda mudança relevante (`OrderCreated`, `OrderStatusChanged`, `DeliveryRegistered`) **também** persiste um `Notification` e emite `Notification` via SignalR — o frontend incrementa `unreadCount()`.

### Sugestões de testes

**Backend (xUnit + FluentAssertions + Moq):**
- **Domain**: `Order.ChangeStatus` cobrindo todas as transições válidas/inválidas; `Order.MarkAsDelivered` (Created/Processing/Shipped → Delivered, Delivered → noop, Cancelled → DomainException).
- **Application/Auth**: `LoginCommandHandler` (sucesso, senha errada, re-hash transparente para hash legacy); `RefreshTokenCommandHandler` (rotação, expirado, reuso revoga família).
- **Application/Orders**: `CreateOrderCommandHandler` (CEP inválido → DomainException; `orderNumber` duplicado → ConflictException).
- **Application/Validation**: cada `Validator` cobrindo todas as regras (regex, `MaximumLength`, `NotEmpty`).
- **Infrastructure**: testes de integração contra Mongo em container (`Testcontainers`).

**Frontend (Vitest preferencialmente, ou Jasmine/Karma do scaffold padrão):**
- **Services**: `AuthService` (persistência localStorage, parse de JWT, refresh path); `RealtimeService` (emit de `events$` por evento).
- **Interceptors**: `errorInterceptor` (401 dispara refresh; reuso de fila não duplica request; opt-out em rotas `/auth/*`).
- **Componentes**: `OrdersComponent` (form inválido bloqueia submit; modal de status filtra transições válidas); `DashboardComponent` (cards clicáveis sincronizam select; grid recarrega em eventos SignalR).

**End-to-end (Playwright sugerido):**
- Fluxo completo "registrar → logar → criar pedido → mudar status → registrar entrega → ver dashboard atualizar em tempo real".

---

> Documento mantido vivo. Para padrões de código e regras de evolução.
