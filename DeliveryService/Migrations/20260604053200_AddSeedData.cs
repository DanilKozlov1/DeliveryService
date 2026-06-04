using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeliveryService.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Все" },
                    { 2, "Напитки" },
                    { 3, "Десерты" },
                    { 4, "Горячее" }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Created_At", "Email", "Name", "Password", "Phone", "Role" },
                values: new object[] { 1, new DateTime(2026, 6, 4, 5, 31, 59, 922, DateTimeKind.Utc).AddTicks(7568), "admin@delivery.ru", "admin", "admin", "00000000000", "admin" });

            migrationBuilder.InsertData(
                table: "Foods",
                columns: new[] { "Id", "CategoriesId", "Description", "ImageUrl", "Price", "Title", "Weight" },
                values: new object[,]
                {
                    { 1, 2, "Ароматный американо", "pack://application:,,,/Images/coffee.png", 150.00m, "Кофе", 200 },
                    { 2, 3, "Нежный сливочный чизкейк", "pack://application:,,,/Images/cheesecake.png", 350.00m, "Чизкейк", 150 },
                    { 3, 3, "Свежий слоёный круассан", "pack://application:,,,/Images/croissant.png", 180.00m, "Круассан", 100 },
                    { 4, 4, "Узбекский плов с бараниной", "pack://application:,,,/Images/plov.png", 450.00m, "Плов", 400 },
                    { 5, 4, "Борщ со сметаной", "https://images.unsplash.com/photo-1547592180-85f173990554?w=400", 320.00m, "Борщ", 400 },
                    { 6, 2, "Кофе латте с молоком", "https://images.unsplash.com/photo-1561047029-3000c68339ca?w=400", 200.00m, "Латте", 250 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
