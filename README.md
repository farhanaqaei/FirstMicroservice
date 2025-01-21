# FirstMicroservice

A hands-on exploration project focusing on microservices implementation and container orchestration concepts.

## Learning Focus

- Microservices patterns and practices
- Container lifecycle management
- Service orchestration
- Inter-service communication
- Scalability and resilience
- Observability and monitoring

## Technology Stack

- .NET Core
- Docker
- Kubernetes
- RESTful APIs
- Message Queues
- Service Discovery
- API Gateway

## Implementation Goals

1. Service Development
   - Basic service implementation
   - API endpoints
   - Service configuration

2. Containerization
   - Docker image optimization
   - Multi-stage builds
   - Container networking

3. Orchestration
   - Kubernetes deployment
   - Service scaling
   - Load balancing
   - Health monitoring

4. Communication Patterns
   - Synchronous (HTTP/REST)
   - Asynchronous (Message Queues)
   - Service discovery

## Getting Started

1. Clone the repository
```bash
git clone https://github.com/farhanaqaei/FirstMicroservice.git
```

2. Build and run locally
```bash
dotnet build
dotnet run
```

3. Deploy to Kubernetes
```bash
kubectl apply -f k8s/platforms-depl.yaml
kubectl apply -f k8s/platforms-np-srv.yaml          
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Notes

This project prioritizes learning microservices concepts and container orchestration. The focus is on understanding distributed systems rather than implementing specific architectural patterns.

Documentation will be updated as the project evolves.
