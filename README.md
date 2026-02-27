# CqrsMediatRDemo

A professional sample project based on **Clean Architecture + CQRS + MediatR + Domain Events + Outbox Pattern**

## Technologies

* .NET 10
* ASP.NET Core Web API
* MediatR 14.0.0
* Entity Framework Core
* Clean Architecture

## Day 1: Implemented Project Structure

* CqrsMediatRDemo.Domain
* CqrsMediatRDemo.Application
* CqrsMediatRDemo.Infrastructure
* CqrsMediatRDemo.Api

## Day 2 – Domain Model (Domain Layer)

* Implementing a base Entity
* Implementing a base Value Object + a sample `Money` value object
* Aggregate Root: `Product` with protected invariants

## Day 3 – Commands with MediatR

* Implement `CreateProductCommand` (record)
* Add a Validator using FluentValidation
* Create the initial Handler (with simulated persistence)
* Register Validators in Dependency Injection (DI)

## Day 4 – Queries with MediatR

* Implement `GetProductByIdQuery` and its Handler with projection to `ProductDto`
* Introduce `ListProductsQuery` (optional)
* Focus on read-only operations and optimization (`AsNoTracking` in the final version)

## Day 5 – Domain Events

* Define a base `DomainEvent` and an example `ProductPriceChangedEvent`
* Implement event collection mechanism in the Aggregate Root (`Product`)
* Publish events synchronously using MediatR `INotification`
* Sample Handler using `ILogger` to react to the event

## Day 6 – Outbox Pattern

* Define the `OutboxMessages` table entity and its configuration
* Implement `OutboxInterceptor` to extract and store Domain Events within the transaction
* Create a simple Background Service for polling and processing messages
* Ensure event reliability without losing any messages

## Day 7.1 – Write Side with SQL Server

* Real `WriteDbContext` using EF Core + SQL Server
* Configuration for `Product` (including Value Object ownership)
* Initial migration and `Update-Database`
* Outbox Interceptor remains active

## Day 7.2 – Read Side with Elasticsearch

* Use `Elastic.Clients.Elasticsearch` (the new official client)
* Define `ProductReadModel` (denormalized for read operations)
* Create an Index with explicit mapping (`keyword` for filtering, `text` for search)
* Implement `ElasticsearchService` to manage the Index and initial indexing

## Day 7.3 – Synchronization from Outbox to Elasticsearch

* Update `OutboxProcessor` to perform polymorphic deserialization of Domain Events
* Publish events via MediatR + directly update the Read Model in Elasticsearch
* Implement eventual consistency flow: Write → Outbox → ES Read Model
* Initial support for `ProductPriceChangedEvent` and `ProductCreatedEvent` (optional)

## Day 7.4 – Connecting Handlers and Testing Consistency

* **Command Handler:** Persist data in `WriteDbContext` (SQL Server)
* **Query Handler:** Read data from Elasticsearch
* **Simple Controller** for practical testing (POST / GET)
* **Manual eventual consistency test:** Create product → short delay → verify in Read side
* Explanation of **Eventual Consistency** in CQRS with Outbox

## Day 8 – Behaviors and Pipeline in MediatR

* **ValidationBehavior** using FluentValidation
* **LoggingBehavior** using `ILogger` with execution timing
* **CachingBehavior** using `IMemoryCache` (for Queries)
* Registering open-generic behaviors in MediatR
