using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppChat.Migrations
{
    /// <inheritdoc />
    public partial class RefactorContactssDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 1, 7 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 1, 8 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 8,
                column: "UserIdContactB",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 9,
                column: "UserIdContactB",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 10,
                column: "UserIdContactB",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 2, 7 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 12,
                column: "UserIdContactB",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 13,
                column: "UserIdContactB",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 14,
                column: "UserIdContactB",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 4, 5 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 16,
                column: "UserIdContactB",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 17,
                column: "UserIdContactB",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 5, 7 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 6, 8 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 20,
                column: "UserIdContactA",
                value: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 2, 3 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 8,
                column: "UserIdContactB",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 9,
                column: "UserIdContactB",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 10,
                column: "UserIdContactB",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 3, 1 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 12,
                column: "UserIdContactB",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 13,
                column: "UserIdContactB",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 14,
                column: "UserIdContactB",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 3, 8 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 16,
                column: "UserIdContactB",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 17,
                column: "UserIdContactB",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 4, 5 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "UserIdContactA", "UserIdContactB" },
                values: new object[] { 4, 6 });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 20,
                column: "UserIdContactA",
                value: 4);

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "UserIdContactA", "UserIdContactB" },
                values: new object[,]
                {
                    { 21, 5, 1 },
                    { 22, 5, 2 },
                    { 23, 5, 4 },
                    { 24, 5, 6 },
                    { 25, 5, 7 },
                    { 26, 6, 1 },
                    { 27, 6, 2 },
                    { 28, 6, 4 },
                    { 29, 6, 5 },
                    { 30, 6, 8 },
                    { 31, 7, 2 },
                    { 32, 7, 3 },
                    { 33, 7, 5 },
                    { 34, 7, 8 },
                    { 35, 7, 1 },
                    { 36, 8, 3 },
                    { 37, 8, 4 },
                    { 38, 8, 6 },
                    { 39, 8, 7 },
                    { 40, 8, 1 }
                });
        }
    }
}
