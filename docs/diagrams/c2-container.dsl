workspace "NutriMetrics - Containers" "Container view" {

    model {
        user = person "User" "Person who logs consumed food items and checks their calorie intake"

        nutriMetrics = softwareSystem "NutriMetrics" "Multi-user platform for calorie and nutrition tracking" {
            webApp = container "Web App" "React SPA for visualizing consumption by date range, in tables and charts" "React" {
                tags "React"
            }

            api = container "NutriMetrics.Api" ".NET 10 API, Clean Architecture + CQRS, JWT authentication" "ASP.NET Core 10"

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
        api -> database "Reads/Writes" "EF Core"
        api -> translationProvider "Translates the query" "HTTPS/JSON"
        api -> calorieNinjas "Retrieves nutritional data" "HTTPS/JSON"
    }

    views {
        container nutriMetrics "Containers" {
            include *
            autoLayout lr
        }

        styles {
            element "Person" {
                shape Person
                background #0078D4
                color #ffffff
            }
            element "Software System" {
                background #0078D4
                color #ffffff
            }
            element "Container" {
                background #2B88D8
                color #ffffff
            }
            element "Database" {
                shape Cylinder
                background #1F8A70
                color #ffffff
            }
            element "React" {
                background #61DAFB
                color #000000
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