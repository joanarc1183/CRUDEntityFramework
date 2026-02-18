using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalPatientManager.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeDoctorSeedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "FullName",
                value: "Maya Putri");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "FullName",
                value: "Raka Wijaya");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "FullName",
                value: "Nanda Lestari");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "FullName",
                value: "Dr. Maya Putri");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "FullName",
                value: "Dr. Raka Wijaya");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "FullName",
                value: "Dr. Nanda Lestari");
        }
    }
}

