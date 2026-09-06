# VKPolls Authentication Microservice - Agent Instructions

## 📋 Project Overview

**VKPolls Auth Microservice** is a .NET 9/10 ASP.NET Core authentication microservice built with clean architecture principles. It manages user registration, login, OTP verification, and email verification for the VKPolls platform.

### Technology Stack
- **.NET Versions**: .NET 9 (Domain, Application) and .NET 10 (Web, Identity, Infrastructure, ApiClient)
- **Architecture Pattern**: Clean Architecture with layered design
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server
- **API Documentation**: Swagger/OpenAPI
- **Dependency Injection**: Built-in .NET DI Container

### Git Repository
- **Remote**: https://github.com/Vasanth01082001/vkpolls.auth
- **Branch**: development
- **Location**: C:\Users\Vasanth Kumar\source\repos\vkpolls_microservices\vkpolls.auth\

---

## 🏗️ Project Structure & Architecture

### Solution Organization (6 Projects)

The solution is organized into 3 logical groups:

#### **1. Core Layer** (Domain & Application Logic)

##### **vkpolls.auth.Domain** (.NET 9)
**Purpose**: Business entities and enums  
**Key Files**:
- `User.cs` - Core user entity with properties:
  - `UserId` (int, PK)
  - `UserIdentityId` (string, FK to AspNetUser)
  - `ProfilePicUrl`, `Name`, `FirstName`, `LastName`, `Gender`, `DOB`
  - `CanCreatePoll`, `IsActive`, `NoOfPollsCreated`
- `Gender.cs` - Gender enum

**Notes**: 
- The `User` entity is NOT used in current auth flow (ASP.NET Identity user is primary)
- Domain models are intentionally minimal for clean separation

---

##### **vkpolls.auth.Application** (.NET 9)
**Purpose**: Application contracts (interfaces), DTOs, and custom exceptions  
**Directory Structure**:
- `Contracts/Identity/`
  - `IAuthService.cs` - Contract for auth operations (Login, Register)
  - `IAuthConfirmation.cs` - Contract for OTP/Email verification
  - `IGenerateUserNameService.cs` - Username generation logic
- `Contracts/Logger/`
  - `IAppLogger.cs` - Logging abstraction
- `Exceptions/`
  - `BadRequestException.cs`
  - `NotFoundException.cs`
  - `ValidationException.cs`
- `Models/`
  - `UserAuthIdentity.cs` - Login/Register request DTO (identifier, password)
  - `OtpVerify.cs` - OTP verification request (phoneNumber, otpCode)
  - `EmailVerify.cs` - Email verification request (email, token)
  - `OtpTokenVerify.cs` - OTP token verification request

**Key Principles**:
- No business logic here; purely contracts and models
- Exceptions inherit from custom base exceptions
- All DTOs use public auto-properties with C# 10 records recommended

---

#### **2. Infrastructure Layer**

##### **vkpolls.auth.Identity** (.NET 10)
**Purpose**: Identity management using ASP.NET Core Identity  
**Directory Structure**:
- `DbContext/`
  - `VKPollsIdentityDbContext.cs` - IdentityDbContext inheriting from IdentityDbContext<IdentityUser>
	- Custom indices on Email and PhoneNumber (unique, nullable-aware)
	- Migrations folder with migration history
- `Services/`
  - `AuthService.cs` - Implementation of IAuthService
	- `RegisterAsync()` - User registration with phone/email validation
	- `LoginAsync()` - Login via email or 10-digit phone, with confirmation checks
  - `AuthConfirmation.cs` - Implementation of IAuthConfirmation
	- `VerifyOtpAsync()` - Phone number OTP verification
	- `VerifyEmailAsync()` - Email link verification
	- `VerifyOtpTokenAsync()` - MSG91 OTP token verification
  - `GenerateUserNameService.cs` - Username auto-generation service
- `IdentityServiceRegistration.cs` - Dependency injection setup
  - DbContext configuration
  - Identity options (password policy, confirmation requirements)
  - Service registration

**Password Policy**:
- Min length: 6 characters
- Require uppercase, lowercase, digits
- No non-alphanumeric required

**Database Schema**:
- Uses ASP.NET Core Identity tables (AspNetUsers, AspNetRoles, etc.)
- Custom indices: `IX_AspNetUsers_Email` (unique), `IX_AspNetUsers_PhoneNumber` (unique)
- Email and PhoneNumber columns nullable with filtered unique indices

---

##### **vkpolls.auth.Infrastructure** (.NET 10)
**Purpose**: Cross-cutting concerns and utilities  
**Files**:
- `InfrastructureServiceRegistration.cs` - Registers logging and other infrastructure services
- `LoggerAdaptor.cs` - Adapts ILogger<T> to custom IAppLogger interface

---

##### **VkPolls.Api** (.NET 10) - **[ApiClient Library]**
**Purpose**: External API client for SMS, Email, and OTP services  
**Directory Structure**:
- `Configuration/`
  - `ApiEndpoints.cs` - Centralized endpoint constants
	- SMS: `api/Otp/SendOTP`
	- Email: `api/Email/SendVerification`
	- MSG91 OTP Token: `api/v5/widget/verifyAccessToken`
  - `ApiClientOptions.cs` - Configuration POCO (BaseUrl, ApiKey, RequestTimeoutSeconds)
- `Abstraction/` (Interfaces)
  - `IApiClient.cs` - Generic HTTP client (GetAsync<T>, PostAsync<T>, DeleteAsync)
  - `ISmsService.cs` - Contract for SMS operations
  - `IEmailService.cs` - Contract for email operations
  - `IMSG91Service.cs` - Contract for MSG91 OTP verification
- `Services/` (Implementations)
  - `ApiClient.cs` - Generic HTTP client using HttpClient
	- Constructs full URLs from endpoints and base URL
	- Handles JSON serialization/deserialization
	- Calls `EnsureSuccessStatusCode()` for error handling
  - `SmsService.cs` - SMS sending via external API
	- Extension method: `AddSmsService()`
	- Returns SMSStatus response
  - `EmailService.cs` - Email verification sending
	- Extension method: `AddEmailService()`
	- Returns EmailStatus response
  - `MSG91Service.cs` - MSG91 OTP token verification
	- Extension method: `AddOtpService()` (different from AddSmsService)
	- Calls VerifyOtpToken endpoint
- `ApiClientServiceRegistration.cs` - DI setup for ApiClient

**Configuration Keys** (appsettings.json):
```json
{
  "ApiClients": {
	"BaseUrl": "https://external-api.example.com",
	"ApiKey": "api-key-here",
	"RequestTimeoutSeconds": 30
  },
  "MSG91ApiClient": {
	"BaseUrl": "https://msg91-api.example.com",
	"ApiKey": "msg91-api-key",
	"RequestTimeoutSeconds": 30
  }
}
```

---

#### **3. Presentation Layer**

##### **vkpolls.auth.Api** (.NET 10) - **[Web API Project]**
**Project File**: `vkpolls.auth.Web.csproj`  
**Purpose**: ASP.NET Core REST API entry point

**Directory Structure**:
- `Program.cs` - Application startup
  - Middleware registration (Exception, Auth, HTTPS)
  - Service registration (Identity, Infrastructure, ApiClient, Email, SMS)
  - Swagger configuration
  - Controller mapping
- `Controllers/`
  - `User/AuthController.cs` - Authentication endpoints
	- `POST /api/auth/Register` - Register new user
	- `POST /api/auth/Login` - User login
	- `POST /api/auth/VerifyOtp` - Verify phone OTP
	- `GET /api/auth/VerifyEmail` - Verify email link
	- `POST /api/auth/VerifyOtpToken` - Verify MSG91 OTP token
- `Middlewares/`
  - `ExceptionMiddleware.cs` - Global exception handling
	- Maps custom exceptions to HTTP responses
	- Returns `CustomProblemDetails` with appropriate status codes
- `Models/`
  - `CustomProblemDetails.cs` - RFC 7807 Problem Details format

**Swagger Routes**:
- API Base: `/api`
- UI: `/swagger`
- JSON: `/swagger/v1/swagger.json`

---

## 🔄 Authentication Flow

### User Registration Flow
```
POST /api/auth/Register
  ↓
AuthService.RegisterAsync()
  ├─ Validate email/phone format
  ├─ Generate unique username (GenerateUserNameService)
  ├─ Create IdentityUser
  ├─ Send OTP via SmsService
  └─ Return success
```

### User Login Flow
```
POST /api/auth/Login
  ↓
AuthService.LoginAsync()
  ├─ Parse identifier (email or 10-digit phone)
  ├─ Find user by email or phone
  ├─ Check EmailConfirmed/PhoneNumberConfirmed
  ├─ Validate password
  └─ Return success or error
```

### Phone OTP Verification
```
POST /api/auth/VerifyOtp
  ↓
AuthConfirmation.VerifyOtpAsync()
  ├─ Find user by phone
  ├─ Verify token using UserManager.VerifyChangePhoneNumberTokenAsync()
  ├─ Set PhoneNumberConfirmed = true
  └─ Return success
```

### Email Verification
```
GET /api/auth/VerifyEmail?email=...&token=...
  ↓
AuthConfirmation.VerifyEmailAsync()
  ├─ Find user by email
  ├─ Confirm email using UserManager.ConfirmEmailAsync()
  └─ Return success
```

### MSG91 OTP Token Verification
```
POST /api/auth/VerifyOtpToken
  ↓
AuthConfirmation.VerifyOtpTokenAsync()
  ├─ Call MSG91Service.VerifyAccessTokenAsync()
  ├─ ApiClient calls MSG91 API
  └─ Return token validity
```

---

## 🔧 Key Services & Contracts

### IAuthService Interface
```csharp
interface IAuthService {
	Task LoginAsync(UserAuthIdentity userAuthIdentity);
	Task RegisterAsync(UserAuthIdentity userAuthIdentity);
}
```

### IAuthConfirmation Interface
```csharp
interface IAuthConfirmation {
	Task VerifyOtpAsync(OtpVerify otpVerify);
	Task VerifyEmailAsync(EmailVerify emailVerify);
	Task VerifyOtpTokenAsync(OtpTokenVerify otpTokenVerify);
}
```

### IApiClient Interface
```csharp
interface IApiClient {
	Task<T?> GetAsync<T>(string url, object? query = null);
	Task<T?> PostAsync<T>(string url, object? body = null);
	Task<bool> DeleteAsync(string url);
}
```

---

## ⚙️ Dependency Injection Setup

### Program.cs Service Registration Order
1. **Controllers** - `AddControllers()`
2. **Identity Services** - `AddIdentityServices(configuration)` from IdentityServiceRegistration
3. **Infrastructure** - `AddInfrastructureServices()` from InfrastructureServiceRegistration
4. **SMS Service** - `AddSmsService(configuration)` from VkPolls.Api
5. **Email Service** - `AddEmailService(configuration)` from VkPolls.Api
6. **Authorization** - `AddAuthorization()`
7. **Swagger** - `AddEndpointsApiExplorer()`, `AddSwaggerGen()`

### Middleware Pipeline
```
ExceptionMiddleware
  ↓
HTTPS Redirection
  ↓
Authentication
  ↓
Authorization
  ↓
Controller Routing
```

---

## 📊 Database Schema

### ASP.NET Core Identity Tables (Modified)
- **AspNetUsers** - Base user table
  - Identity columns: Id, UserName, Email, PhoneNumber
  - Indices: Email (unique), PhoneNumber (unique, nullable-aware)
  - Fields used:
	- `Email` - User email
	- `PhoneNumber` - 10-digit phone number
	- `EmailConfirmed` - Email verification status
	- `PhoneNumberConfirmed` - Phone verification status
	- `PasswordHash` - Hashed password

### Note on Domain.User Entity
- **Not currently integrated** into the auth flow
- Exists in Domain layer but unused
- Future: May be used for user profile data in polls service
- Current IdentityUser from ASP.NET Core Identity serves as auth user

---

## 🔐 Security Considerations

### Password Policy
- Minimum 6 characters
- Uppercase required
- Lowercase required
- Digits required
- Non-alphanumeric NOT required

### Confirmation Requirements
- Email confirmation required for login
- Phone number confirmation required for login
- OTP/Email tokens generated and verified by UserManager

### Unique Constraints
- Email: Unique (nullable-aware index)
- PhoneNumber: Unique (nullable-aware index)

### Exception Handling
- Global exception middleware catches all exceptions
- Maps to RFC 7807 Problem Details format
- Custom exceptions: BadRequestException, NotFoundException, ValidationException

---

## 🚀 Common Tasks & Patterns

### Adding a New Authentication Endpoint
1. Create request/response DTOs in `vkpolls.auth.application/Models/`
2. Add contract method to `IAuthService` or `IAuthConfirmation`
3. Implement in `AuthService.cs` or `AuthConfirmation.cs`
4. Add controller action in `AuthController.cs`
5. Update tests if applicable

### Integrating a New External Service
1. Create interface in `VkPolls.Api/Abstraction/` (e.g., `INewService.cs`)
2. Add endpoint constant to `ApiEndpoints.cs`
3. Implement service in `VkPolls.Api/Services/` with extension method
4. Register in `Program.cs` using extension method
5. Inject into controller/service as needed

### Database Migration
1. Update DbContext entity configuration in `VKPollsIdentityDbContext.cs`
2. Run: `dotnet ef migrations add <MigrationName> -p vkpolls.auth.Identity -s vkpolls.auth.Api`
3. Run: `dotnet ef database update -p vkpolls.auth.Identity -s vkpolls.auth.Api`

### Adding Custom Validation Exception
1. Create exception class inheriting from base (or create new base)
2. Add to `vkpolls.auth.application/Exceptions/`
3. Add handler to `ExceptionMiddleware.cs` if custom behavior needed
4. Throw from service methods

---

## 📁 File Locations Quick Reference

| Purpose | Location | File(s) |
|---------|----------|---------|
| API Entry Point | vkpolls.auth.Api | Program.cs |
| Auth Endpoints | vkpolls.auth.Api | Controllers/User/AuthController.cs |
| Auth Service Logic | vkpolls.auth.Identity | Services/AuthService.cs |
| Email/OTP Verification | vkpolls.auth.Identity | Services/AuthConfirmation.cs |
| Database Context | vkpolls.auth.Identity | DbContext/VKPollsIdentityDbContext.cs |
| Auth Contracts | vkpolls.auth.application | Contracts/Identity/IAuthService.cs |
| Verification Contracts | vkpolls.auth.application | Contracts/Identity/IAuthConfirmation.cs |
| DTOs/Models | vkpolls.auth.application | Models/*.cs |
| Custom Exceptions | vkpolls.auth.application | Exceptions/*.cs |
| API Client | VkPolls.Api | ApiClient.cs |
| SMS Service | VkPolls.Api | Services/SmsService.cs |
| Email Service | VkPolls.Api | Services/EmailService.cs |
| OTP Service | VkPolls.Api | Services/MSG91Service.cs |
| API Endpoints | VkPolls.Api | Configuration/ApiEndpoints.cs |
| Exception Middleware | vkpolls.auth.Api | Middlewares/ExceptionMiddleware.cs |
| Domain Entities | vkpolls.auth.Domain | User.cs, Gender.cs |

---

## 🎯 Configuration Files

### appsettings.json (expected structure)
```json
{
  "ConnectionStrings": {
	"VKPollsIdentityConnection": "Server=...;Database=VKPollsIdentity;..."
  },
  "ApiClients": {
	"BaseUrl": "https://api.example.com",
	"ApiKey": "your-api-key",
	"RequestTimeoutSeconds": 30
  },
  "MSG91ApiClient": {
	"BaseUrl": "https://msg91.example.com",
	"ApiKey": "your-msg91-key",
	"RequestTimeoutSeconds": 30
  }
}
```

---

## 🔍 Debugging Tips

### Check User Registration Issues
- Look at `AuthService.RegisterAsync()` for validation logic
- Check unique constraint violations on Email/PhoneNumber
- Verify SMS service is configured and reachable

### Check Login Failures
- Verify EmailConfirmed or PhoneNumberConfirmed status
- Check password hash using UserManager
- Ensure user exists by email or phone number

### Check OTP Verification Issues
- Verify OTP token generation by UserManager
- Check token expiration (default 24 hours)
- Ensure phone number matches exactly

### Check External API Integration
- Verify ApiEndpoints constants match actual endpoints
- Check ApiClientOptions configuration in appsettings.json
- Add logging to ApiClient.cs for debugging HTTP requests
- Validate API keys and base URLs

### View Logs
- Enable detailed logging in appsettings (Development)
- Check Application Insights if configured
- Review ExceptionMiddleware responses

---

## 📝 Naming Conventions

### Classes
- Service implementations: `<Feature>Service.cs` (e.g., AuthService, SmsService)
- Contracts: `I<Feature>Service.cs` (e.g., IAuthService, ISmsService)
- Extension methods: `<Feature>ServiceExtensions` in service file
- DbContext: `<ProjectName>DbContext.cs` (e.g., VKPollsIdentityDbContext)

### Methods
- Async operations: `<Action>Async()` (e.g., RegisterAsync, VerifyOtpAsync)
- Boolean returns: `Is<State>()` or `Can<Action>()` (e.g., IsActive)
- Property accessors: Standard C# conventions

### Folders
- Services: `Services/`
- Contracts: `Contracts/`
- Models: `Models/`
- DbContext: `DbContext/`
- Configuration: `Configuration/`
- Controllers: `Controllers/`
- Middlewares: `Middlewares/`

---

## 🐛 Known Issues & Quirks

1. **Domain.User Entity Unused**: The User entity in Domain layer is not integrated into current auth. Future refactoring may use it.
2. **AddOtpService vs AddSmsService**: MSG91 service uses `AddOtpService()` extension, SMS uses `AddSmsService()`. Different naming for clarity.
3. **ApiClient Configuration**: SMS and Email services read from "ApiClients" config section, MSG91 reads from "MSG91ApiClient".
4. **IdentityUser Confirmation**: Login requires BOTH email and phone confirmation checks, which may need refinement per business logic.

---

## 📚 Related Microservices

This is part of the **vkpolls_microservices** architecture:
- **vkpolls.auth** (this service) - Authentication & Identity
- Other microservices: Polls service, Notifications service, etc.

### Inter-service Communication
- Auth service exposes user authentication endpoints
- Other services can verify tokens using Auth endpoints
- Shared domain events or messages (if applicable)

---

## ✅ Verification Checklist Before Committing

- [ ] Code follows naming conventions above
- [ ] All custom exceptions are caught by middleware
- [ ] New services are registered in Program.cs
- [ ] Configuration keys are documented in this file
- [ ] Database migrations are generated and committed
- [ ] Async/await is used correctly (no blocking calls)
- [ ] Null checks and validation are in place
- [ ] Tests pass (if applicable)
- [ ] API endpoints are documented in Swagger

---

## 📞 Contact & Support

- **Repository**: https://github.com/Vasanth01082001/vkpolls.auth
- **Branch**: development
- **IDE**: Visual Studio Community 2026 (18.6.3)
- **.NET Versions**: 9 & 10

---

**Last Updated**: November 2024  
**Version**: 1.0  
**Maintained By**: Development Team
