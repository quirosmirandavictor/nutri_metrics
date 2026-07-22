using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateFoodItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Calories = table.Column<double>(type: "double", precision: 10, scale: 2, nullable: false),
                    ProteinGrams = table.Column<double>(type: "double", precision: 10, scale: 2, nullable: false),
                    FatGrams = table.Column<double>(type: "double", precision: 10, scale: 2, nullable: false),
                    CarbohydratesGrams = table.Column<double>(type: "double", precision: 10, scale: 2, nullable: false),
                    ServingSizeGrams = table.Column<double>(type: "double", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodItems");
        }
    }
}
