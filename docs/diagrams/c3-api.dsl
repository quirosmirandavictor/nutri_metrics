workspace "NutriMetrics - API Components" "Component view of NutriMetrics.Api" {

    model {
        user = person "User" "Person who logs consumed food items and checks their calorie intake"

        nutriMetrics = softwareSystem "NutriMetrics" "Multi-user platform for calorie and nutrition tracking" {
            webApp = container "Web App" "React SPA for visualizing consumption by date range, in tables and charts" "React" {
                tags "React"
            }

            api = container "NutriMetrics.Api" ".NET 10 API, Clean Architecture + CQRS, JWT authentication" "ASP.NET Core 10" {
                identityModule = component "Identity Module" "Registration, login and JWT issuance" "Clean Architecture + CQRS"
                calorieTrackingModule = component "CalorieTracking Module" "Food item registration and search, calorie calculation" "Clean Architecture + CQRS"
            }

            database = container "MySQL Database" "nutrimetrics_calorietracking: Identity and CalorieTracking tables" "MySQL 8.0" {
                tags "Database"
            }
        }

        calorieNinjas = softwareSystem "CalorieNinjas API" "External nutritional data provider" {
            tags "External"
        }

        translationProvider = softwareSystem "Translation Provider" "Translates food search queries from Spanish to English" {
            tags "External"
        }

        user -> webApp "Logs food items and views reports" "HTTPS"
        webApp -> api "Consumes" "JSON/HTTPS"

        identityModule -> database "Reads/Writes users and roles" "EF Core"
        calorieTrackingModule -> database "Reads/Writes logged food items" "EF Core"
        calorieTrackingModule -> translationProvider "Translates the query" "HTTPS/JSON"
        calorieTrackingModule -> calorieNinjas "Retrieves nutritional data" "HTTPS/JSON"
    }

    views {
        component api "ApiComponents" {
            include *
            autoLayout lr
        }

        styles {
            element "Person" {
                shape Person
                background #0078D4
                color #ffffff
            }
            element "Container" {
                background #2B88D8
                color #ffffff
            }
            element "Component" {
                background #85C1E9
                color #000000
            }
            element "Database" {
                shape Cylinder
                background #1F8A70
                color #ffffff
            }
            element "External" {
                background #6c757d
                color #ffffff
            }
            relationship "Relationship" {
                color #0078D4
                thickness 3
            }
        }
    }
}