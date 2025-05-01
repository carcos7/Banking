# AntiFraudSystem
AntiFraudSystem is a modular .NET-based solution designed to detect and prevent fraudulent banking transactions. It comprises two primary services: TransactionService for handling transaction processing and AntiFraudService for fraud detection. The system is built with a clean architecture approach, ensuring scalability and maintainability.​

Features
Transaction Processing: Handles the creation and management of banking transactions.

Fraud Detection: Analyzes transactions to identify potential fraudulent activities.

Modular Architecture: Separation of concerns with distinct layers for API, Application, Domain, and Infrastructure.

Dockerized Deployment: Includes Docker Compose configuration for easy setup and deployment
## 🌐 Architecture

### Core Components
| Service               | Port  | Description                     |
|-----------------------|-------|---------------------------------|
| Transaction Service   | 5050  | Processes financial transactions|
| Anti-Fraud Service    | 5051  | Validates transaction legitimacy|

## 🔄 Transaction Validation Flow

```mermaid
sequenceDiagram
    participant C as Client
    participant T as Transaction Service:5050
    participant K as Kafka
    participant A as Anti-Fraud Service:5051

    C->>+T: POST /transactions
    T->>+K: Publish "transaction.created"
    K->>+A: Consume event
    alt Valid
        A->>+K: Publish "transaction.validated<br>{status: APPROVED}"
    else Fraud
        A->>+K: Publish "transaction.validated<br>{status: REJECTED}"
    end
    K-->>-T: Consume validation result
    T-->>-C: Return final status
```

## ▶️ Running the System
```bash
docker-compose up -d
```
