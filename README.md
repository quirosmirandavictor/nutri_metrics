# 🥗 Nutri Metrics Calorie Tracking Module

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Architecture: Clean](https://img.shields.io/badge/Architecture-Clean-1F8A70)](README.md#-architecture)
[![Pattern: CQRS](https://img.shields.io/badge/Pattern-CQRS%20%2B%20MediatR-blue)](README.md#-architecture)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

# 📖 Overview

**NutriMetrics** is a modular platform built on **.NET 10** designed for calorie and nutritional tracking.

This repository contains the **CalorieTracking** module, which allows users to search for nutritional information by entering free-text food descriptions in **Spanish** (e.g., *"2 manzanas y 100g de pechuga de pollo"*).

The system seamlessly translates the input query using **GoogleTranslateFreeApi** and queries the **CalorieNinjas** database, returning structured macronutrients (calories, protein, fat, carbohydrates, and serving size) through a decoupled, clean design.

---

# 🏗 Architecture & Clean Design

The project strictly follows **Clean Architecture** principles and **CQRS**, ensuring the domain core remains 100% free of external dependencies.

```mermaid
graph TD
    %% Client & Host Layer
    subgraph Host["NutriMetrics.Api (Presentation Layer)"]
        Client([Client / Postman / cURL])
        Controller[FoodController]
    end

    %% Application Layer (CQRS)
    subgraph Application["NutriMetrics.Modules.CalorieTracking.Application"]
        Mediator{MediatR / ISender}
        Query[SearchFoodQuery]
        Handler[SearchFoodQueryHandler]
        ResponseDTO[FoodItemResponse DTO]
    end

    %% Domain Layer
    subgraph Domain["NutriMetrics.Modules.CalorieTracking.Domain"]
        Entity[FoodItem Entity]
        NutritionContract[["INutritionApiClient (Interface)"]]
        TranslationContract[["ITranslationService (Interface)"]]
    end

    %% Infrastructure Layer
    subgraph Infrastructure["NutriMetrics.Modules.CalorieTracking.Infrastructure"]
        HttpClient[CalorieNinjasHttpClient]
        TranslateService[GoogleTranslationService]
        ExternalDTOs[CalorieNinjas DTOs]
    end

    %% External Services
    subgraph External["External APIs"]
        GoogleAPI["Google Translate API"]
        CalorieNinjasAPI["CalorieNinjas API"]
    end

    %% Flow Relationships
    Client -->|"1. GET /api/food/search?query=..."| Controller
    Controller -->|"2. Send(SearchFoodQuery)"| Mediator
    Mediator -->|"3. Dispatches to"| Handler
    Handler -->|"4. Calls"| NutritionContract
    
    HttpClient -->|"Implements"| NutritionContract
    TranslateService -->|"Implements"| TranslationContract

    Handler -->|"5. SearchFoodAsync()"| HttpClient
    HttpClient -->|"6. TranslateToEnglishAsync()"| TranslateService
    TranslateService -->|"7. Translation Request"| GoogleAPI
    GoogleAPI -->|"Returns translated text"| TranslateService
    
    HttpClient -->|"8. HTTP GET v1/nutrition"| CalorieNinjasAPI
    CalorieNinjasAPI -->|"Returns JSON"| ExternalDTOs
    ExternalDTOs -->|"9. Deserializes & Maps to"| Entity
    
    Entity -->|"Returns FoodItem Entities"| Handler
    Handler -->|"10. Maps to"| ResponseDTO
    ResponseDTO -->|"Returns IEnumerable"| Controller
    Controller -->|"11. HTTP 200 OK (JSON)"| Client

    %% Styling
    classDef host fill:#e1f5fe,stroke:#0288d1,stroke-width:1.5px;
    classDef app fill:#e8f5e9,stroke:#388e3c,stroke-width:1.5px;
    classDef domain fill:#fff3e0,stroke:#f57c00,stroke-width:2px;
    classDef infra fill:#f3e5f5,stroke:#7b1fa2,stroke-width:1.5px;
    classDef ext fill:#eceff1,stroke:#607d8b,stroke-width:1.5px;

    class Host host;
    class Application app;
    class Domain domain;
    class Infrastructure infra;
    class External ext;
```

# 📂 Solution Structure

```text
NUTRI_METRICS/
│
├── src/
│   ├── Modules/
│   │   └── CalorieTracking/
│   │       ├── NutriMetrics.Modules.CalorieTracking.Application/   # CQRS Queries, Handlers & DTOs
│   │       │   └── FoodItems/
│   │       │       └── Queries/
│   │       │           └── SearchFood/
│   │       │               ├── FoodItemResponse.cs
│   │       │               └── SearchFoodQuery.cs
│   │       │
│   │       ├── NutriMetrics.Modules.CalorieTracking.Domain/        # Domain Entities & Interfaces
│   │       └── NutriMetrics.Modules.CalorieTracking.Infrastructure/  # External Services & Client Implementations
│   │
│   ├── NutriMetrics.Api/                                           # Entry Point Host & Presentation Layer
│   │   ├── Controllers/
│   │   │   └── FoodController.cs
│   │   ├── Properties/
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.json
│   │   ├── NutriMetrics.Api.csproj
│   │   ├── NutriMetrics.Api.http
│   │   └── Program.cs
│   │
│   └── Shared/                                                     # Shared Kernel & Infrastructure Assets
│       ├── NutriMetrics.Shared.Domain/
│       └── NutriMetrics.Shared.Infrastructure/
│
├── .gitignore
├── NutriMetrics.slnx
└── README.md