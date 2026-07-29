workspace "NutriMetrics - Context" "System Context view" {

    model {
        user = person "User" "Person who logs consumed food items and checks their calorie intake"

        nutriMetrics = softwareSystem "NutriMetrics" "Multi-user platform for calorie and nutrition tracking"

        calorieNinjas = softwareSystem "CalorieNinjas API" "External nutritional data provider" {
            tags "External"
        }

        translationProvider = softwareSystem "Translation Provider" "Translates food search queries from Spanish to English" {
            tags "External"
        }

        user -> nutriMetrics "Logs food items and views reports" "HTTPS"
        nutriMetrics -> translationProvider "Translates the query" "HTTPS/JSON"
        nutriMetrics -> calorieNinjas "Retrieves nutritional data" "HTTPS/JSON"
    }

    views {
        systemContext nutriMetrics "SystemContext" {
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