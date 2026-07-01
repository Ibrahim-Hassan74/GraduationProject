# Security Policy

## Supported Versions

Currently, only the latest release of the Wasla platform is actively supported with security updates.

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take the security of the Wasla platform very seriously. If you discover a security vulnerability, we kindly ask that you follow our responsible disclosure policy.

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please email the vulnerability details to **[ibrahimhassan.dev1@gmail.com](mailto:ibrahimhassan.dev1@gmail.com)**.

Include the following information in your report:

- Type of vulnerability (e.g., XSS, SQLi, IDOR, etc.)
- Clear steps to reproduce the vulnerability
- Any potential impact you have identified

You should receive a response within 48 hours acknowledging receipt of your report.

## Responsible Disclosure Policy

- We will work with you to understand and resolve the issue quickly.
- We ask that you do not publish or share the vulnerability until we have patched it and made the release public.
- We will acknowledge your contribution (if desired) once the issue is resolved.

## Security Best Practices

When contributing to this project, please adhere to the following security principles:

- **Never commit secrets:** Passwords, JWT keys, and API tokens must be injected via Environment Variables or User Secrets.
- **Always use Parameterized Queries:** Entity Framework Core handles this by default, but ensure no raw strings are passed into `FromSqlRaw`.
- **Validate Input:** All DTOs should continue to use validation attributes to ensure data integrity before hitting the database.
- **Maintain Dependency Health:** Keep all NuGet packages and Docker base images up to date.
