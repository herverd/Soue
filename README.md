# RPA & Data Ecosystem (.NET 8)

Este ecossistema automatizado demonstra a captura de dados (RPA) e sua disponibilização através de uma API REST, utilizando containers 
Docker para orquestração de serviços.

##  Arquitetura da Solução

A solução foi desenhada seguindo princípios de **Separação de Responsabilidades** e **Resiliência**:

1.  **Rpa.Worker**: Um *Worker Service* em .NET 8 que simula a extração de dados. Ele possui uma política de persistência idempotente, 
	garantindo que a tabela exista antes da inserção.
2.  **Web.Api**: Uma *Minimal API* de alta performance que expõe os dados coletados. Inclui documentação interativa via **Swagger**.
3.  **Persistência**: Utiliza **SQLite** compartilhado via **Docker Volumes**. Esta escolha técnica simplifica a infraestrutura para este escopo, 
	permitindo que ambos os containers acessem o mesmo arquivo de banco de dados (`rpa.db`) de forma síncrona.

## Como Executar (Docker)

### Pré-requisitos
* Docker e Docker Compose instalados.

### Passo a Passo
1. Clone este repositório:
   ```bash
   git clone [https://github.com/SEU_USUARIO/NOME_DO_REPOSITORIO.git](https://github.com/SEU_USUARIO/NOME_DO_REPOSITORIO.git)
   cd NOME_DO_REPOSITORIO
   
2. Inicie o sistema:
	docker-compose up --build
	
3. Acesse os serviços:
	
	Swagger (API): http://localhost:5000

	Endpoint de Dados: http://localhost:5000/api/cotacoes

	Checar se está no ar: http://localhost:5000/health
	
### Melhorias Futuras

Implementação de Mensageria (RabbitMQ) para desacoplar a captura da persistência.

Unit Testing com xUnit e FluentAssertions para as regras de parsing.

Dashboard de monitoramento com Blazor ou Grafana.