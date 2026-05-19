# Distributed Job Orchestrator

## About the Application

Distributed Job Orchestrator is a highly available distributed system designed to receive, schedule, prioritize, and cancel asynchronous jobs reliably.

The primary goal of the application is to process webhook delivery tasks safely and efficiently while ensuring that no data is lost during infrastructure failures, worker crashes, retries, or network instability.

The platform provides features such as:

- Job prioritization
- Delayed and scheduled execution
- Idempotent job creation
- Retry policies
- Failure handling
- Dead-letter queue processing (DLQ)
- Real-time observability and monitoring

This project is intended for studying and testing resilience patterns in distributed systems and asynchronous processing environments.

---

# How to Run

The entire infrastructure is fully containerized using Docker and Docker Compose.

## Requirements

Make sure the following tools are installed on your machine:

- Docker
- Docker Compose

## Start the Application

Run the following command in the root directory of the project:

```bash
docker-compose up -d --build
```

After the containers are initialized, all services will be available locally.

---

# Available Services

| Service | URL |
|---|---|
| API | `http://localhost:5001` |
| RabbitMQ Management | `http://localhost:15672` |
| Seq | `http://localhost:5341` |
| Webhook Simulator | `http://localhost:8080` |

---

# How to Test and Monitor

The system is composed of multiple services that allow you to test, monitor, and validate the orchestration flow.

---

# Authentication

Before using the protected endpoints, you must first generate a JWT token.

## Generate JWT Token

Endpoint:

```http
GET /api/v1/auth/token
```

Base URL:

```text
http://localhost:5001/api/v1/auth/token
```

This endpoint returns a JWT token that must be included in all protected API requests.

---

## Using the Token

Add the token to the `Authorization` header using the Bearer format:

```http
Authorization: Bearer YOUR_TOKEN
```

You can configure this directly in Scalar before testing the remaining endpoints.

---

# Scalar (API Documentation)

Available at:

```text
http://localhost:5001/scalar
```

Scalar provides the interactive API documentation used to test the API endpoints.

---

## Main Endpoints

---

### `POST /api/v1/jobs`

Creates a new job for asynchronous processing.

### Headers

| Header | Description |
|---|---|
| `Authorization` | Bearer JWT token |
| `Idempotency-Key` | Prevents duplicated job creation |

---

### Request Payload

| Field | Type | Description |
|---|---|---|
| `priority` | number | Job priority (`0 = Low`, `1 = Normal`, `2 = High`) |
| `payload` | string | Payload content that will be sent to the webhook |
| `webhookUrl` | string | Target webhook endpoint |
| `scheduledAt` | string (optional) | ISO 8601 date for future execution |

---

### Example Request

```json
{
  "priority": 2,
  "payload": "Webhook payload example",
  "webhookUrl": "http://webhook-simulator/status/200",
  "scheduledAt": "2026-05-20T15:00:00Z"
}
```

---

### `GET /api/v1/jobs`

Returns all jobs stored in the system.

---

### `DELETE /api/v1/jobs/{jobId}`

Requests cancellation of a queued or processing job.

---

# Seq (Observability)

Available at:

```text
http://localhost:5341
```

Seq provides structured logs and real-time observability.

You can use it to:

- Track incoming API requests
- Monitor worker processing
- Inspect retries
- Analyze failures
- Observe timeout handling
- Validate circuit breaker behavior
- Monitor DLQ events

---

# RabbitMQ (Broker)

Available at:

```text
http://localhost:15672
```

## Credentials

```text
Username: guest
Password: guest
```

RabbitMQ is responsible for queue orchestration and delayed message processing.

You can use the dashboard to:

- Monitor queues
- Inspect queued messages
- Track delayed jobs
- Observe retries
- Validate dead-letter queue behavior
- Analyze worker consumption

---

# Webhook Simulator (Resilience Testing)

The project includes a webhook simulator powered by `httpbin` to validate resilience scenarios.

Base URL:

```text
http://localhost:8080
```

When creating a job, use the following URLs in the `webhookUrl` field to simulate different behaviors.

---

# Successful Processing

## HTTP 200 OK

```text
http://webhook-simulator/status/200
```

Simulates a successful webhook execution.

Use this to validate:

- Successful job processing
- Normal worker execution flow
- Completed jobs

---

# Client Errors

## HTTP 400 Bad Request

```text
http://webhook-simulator/status/400
```

Simulates client-side request failures.

Useful for testing:

- Validation errors
- Non-retryable scenarios
- Error logging

---

## HTTP 404 Not Found

```text
http://webhook-simulator/status/404
```

Simulates missing endpoints.

Useful for testing:

- Invalid webhook destinations
- Failure handling
- Logging and observability

---

# Server Errors

## HTTP 500 Internal Server Error

```text
http://webhook-simulator/status/500
```

Simulates permanent server failures.

Useful for validating:

- Retry policies
- Circuit breaker behavior
- Dead-letter queue processing
- Failure observability

---

## HTTP 503 Service Unavailable

```text
http://webhook-simulator/status/503
```

Simulates temporary infrastructure instability.

Useful for testing:

- Transient failures
- Retry strategies
- Resilience policies

---

# Delayed Responses (Timeout Testing)

The simulator also supports delayed responses to test timeout handling.

## Delay for 3 Seconds

```text
http://webhook-simulator/delay/3
```

## Delay for 10 Seconds

```text
http://webhook-simulator/delay/10
```

## Delay for 30 Seconds

```text
http://webhook-simulator/delay/30
```

These endpoints intentionally delay the response before returning a successful status code.

Useful for validating:

- Timeout policies
- Long-running webhook handling
- Worker resilience
- Retry after timeout
- Slow external dependency behavior

---

# Example Full Flow

1. Start the infrastructure using Docker Compose
2. Generate a JWT token
3. Open Scalar
4. Configure the Bearer token
5. Create jobs using different webhook simulator URLs
6. Monitor logs in Seq
7. Monitor queues and retries in RabbitMQ
8. Cancel pending jobs if necessary
9. Validate resilience behavior under failures and delays