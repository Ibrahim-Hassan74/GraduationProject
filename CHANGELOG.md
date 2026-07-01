# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-06-22

### Added

- **Smart Microbus Management**: End-to-end digitization of station workflows, queuing, and passenger booking.
- **Real-time Tracking**: Live GPS broadcasting and OSRM-powered route ETA calculations.
- **SignalR**: 4 distinct WebSocket hubs for Driver Queues, Dashboards, Location Tracking, and Route Tracking.
- **Queue Management**: Automated, transaction-safe driver queue synchronization with a daily midnight reset job via Hangfire.
- **QR Authentication**: AES-GCM encrypted tokens for touchless gate check-in/out.
- **WhatsApp Integration**: OTP pipeline utilizing a custom Node.js/Puppeteer gateway for headless WhatsApp Web automation.
- **Web Dashboard**: Dedicated APIs for an Angular-powered station manager control panel.
- **Mobile App Support**: Tailored REST APIs for Flutter-based Passenger and Driver applications.
- **Docker Support**: Containerized architecture with multi-stage builds and Docker Compose encompassing SQL Server and Redis.
- **Clean Architecture**: Domain-driven layout utilizing the Unit of Work and Generic Repository patterns.
- **JWT Authentication**: Secure role-based access control (RBAC) featuring refresh token rotation and localized responses.
