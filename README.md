# MAV .NET Application

## 1. Introduction

### 1.1 Purpose of this Repository

The intention for this repository is to act as a personal 'Playground' and for the presentation of the use of best practices and design patterns. It will also form reusable code for bootstrapping new projects of course.

This is not a PoC. Nor is it intended as an implementation of an application that would be ready to run in production. This repository is my own personal work and something that I will be looking to enhance with new features over time.

### 1.2. Inspirations

Two of my favourite repositories that I often look to for inspiration are `Modular Monolith with DDD` and `eShop Containers`. Once out of its infancy, I would like this repository to be used by developers of all levels as a source of inspiration in the same way that these have been for me.

**Modular Monolith with DDD**

https://github.com/kgrzybek/modular-monolith-with-ddd

**eShop Reference Application**

https://github.com/dotnet/eShop

## 2. MAV.API

### 2.1. Overview

A .Net Core 8 Web API project with bootstrapping for Docker (& Docker Compose) support.

### 2.2. Key tech stack and concepts

Basic example of `Commands` and `Queries` using MediatR and Refit.

### 2.3. Component Tests

Component tests using the `WebApplicationFactory`.

Implementing mocked `HttpMessageHandler` with the Refit clients to mock HTTP dependencies and to verify requests.

## 3. MAV.EventListener

### 3.1. Overview

A .Net Core 8 project with bootstrapping for Docker (& Docker Compose) support.

Used to support interaction with Azure Service Bus message queues.

### 3.2. Key tech stack and concepts

Implementation of a typed `QueueListener<T>` as a `IHostedService`.

Implementation of a typed `IMessageProcessor<T>` to process individual messages of type `T`.

Includes basic error handling using Retryable and Non-Retryable exceptions to determine how to handle failures (e.g. abandon & retry, deadletter).

### 3.3. Component Tests

Component tests using the `WebApplicationFactory`.

Implementing mocked `HttpMessageHandler` with the Refit clients to mock HTTP dependencies and to verify requests.

Implementing mocked `ServiceBusClient`, `ServiceBusProcessor` and `ServiceBusSender` with provision to track and verify message completion (or abandonment, deadletting).

## 4. MAV.Infrastructure

### 4.1. Overview

Support for Refit API Clients.
Support for Azure ServiceBus Receivers (Processors).
Support for Azure ServiceBus Senders (Publishers).
Support for Source Generation for efficient Serialization.
Support for Application Insights telemetry.

## 5. Future developments

### 5.1. DDD Implementation

Introduction of support for DDD using Aggregates.

This will include but not limited to some of the below behaviours:

 - Business rule validation behaviour
 - Unit of Work transaction behaviour
 - Idempotency behaviour
 - Aggregate root changed behaviour
 - Domain event dispatching behaviour

### 5.2. Event Sourcing

Adding support for event sourcing using Cosmos as the event store.

### 5.3. Replace Polly with `Microsoft.Extensions.Resilience`

The new Microsoft extension looks good. I haven't used this yet so wish to trial it in place of Polly.

### 5.4. Inbox Pattern with Idempotency

Adding support for Idempotency using the inbox pattern.

Possibly using Redis as the store.

### 5.5. Outbox Pattern

Adding support for ensuring delivery of integration events (Inc. internal integration events).

Possibly using MSSQL as the store.

## 6. Deployable Example Project

Presentation of an example implementation using each of the best practices, patterns and concepts as detailed above. This will also be fully deployable into Microsoft Azure.

### 6.1. CI/CD Pipelines supporting Azure AKS

The intention will be to create a working example of a YAML CI Build pipeline executing tests, creating the build artifact(s) and validating the Terraform plan.

High-level intention is to use the likes of the following here:
 - Terraform
 - Skaffold deploy
 - Hiera
 - Helm

### 6.2. BDTest Integration Tests

Addition of a set of integration tests for happy paths as a minimum and utilising the BDTest framework.
