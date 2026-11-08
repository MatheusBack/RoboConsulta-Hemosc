# RoboConsulta Hemosc

Robô em .NET 8 que consulta periodicamente o site do [Hemosc](https://www.hemosc.org.br/), identifica os tipos sanguíneos em falta e registra o resultado em um banco SQLite local.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Como usar

```bash
# Roda continuamente, consultando a cada 30 minutos (padrão)
dotnet run --project src/RoboConsulta

# Roda uma única consulta e encerra (ideal para cron / agendador de tarefas)
dotnet run --project src/RoboConsulta -- cron
```

Os dados ficam na tabela `BloodSearchLogs` do arquivo `blood_search.db`, e os logs em `logs/`.

## Configuração

Em `src/RoboConsulta/appsettings.json`:

| Chave                         | Descrição                          | Padrão                       |
|-------------------------------|------------------------------------|------------------------------|
| `BloodSearch:HemoscUrl`       | Página consultada                  | `https://www.hemosc.org.br/` |
| `BloodSearch:IntervalMinutes` | Intervalo entre consultas (worker) | `30`                         |
| `BloodSearch:DatabasePath`    | Caminho do arquivo SQLite          | `blood_search.db`            |

Qualquer valor pode ser sobrescrito por variável de ambiente, por exemplo `BloodSearch__IntervalMinutes=60`.

## Testes

```bash
dotnet test
```

## Estrutura

```
src/RoboConsulta/
├── Configuration/   opções lidas do appsettings
├── Data/            acesso ao SQLite
├── Models/          MissingBlood, BloodSearchLog
├── Services/        HemoscClient (HTTP), HemoscParser (HTML), BloodSearchJob
└── Workers/         BloodSearchWorker (execução periódica)
tests/RoboConsulta.Tests/
```

## Licença

MIT. Veja `LICENSE.txt`.
