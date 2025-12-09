Absolute Cinema Backend – Architecture Overview(so far)

Caching Layer (Redis)
• Implemented distributed caching using Redis via StackExchange.Redis.
• Added attribute-based caching ([Cached]) to automatically cache controller actions.
• Implemented cache invalidation via [InvalidateCache] attribute.
• Used Redis Insight for real-time cache monitoring.

Logging & Monitoring (Serilog + Elasticsearch + Kibana)
• Centralized logging using Serilog sinks.
• Logs are shipped to Elasticsearch, structured, and searchable.
• Built custom Kibana dashboards for API performance, request logs, status code distribution, and error analysis.
• Created ExceptionMiddleware and LoggingActionFilter for unified error & request monitoring.

Clean Architecture Enhancements
• Introduced AutoMapper profiles for DTO–Entity mapping.
• Implemented global exception handling.
• Added action filters for cross-cutting concerns.
• Ensured strict separation between:
• API (controllers)
• Application (business logic)
• Infrastructure (DB, cache, messaging)

Messaging Layer (RabbitMQ + Saga Patterns)
• Integrated RabbitMQ to prepare for asynchronous workflows.
• Implemented a basic Saga structure for order workflows.
• Configured durable message handling and event publishing.

Dockerized Development Environment
• Fully containerized stack using Docker Compose:
• RabbitMQ
• Redis
• Elasticsearch
• Kibana
• Ability to run the entire infrastructure with a single command:
