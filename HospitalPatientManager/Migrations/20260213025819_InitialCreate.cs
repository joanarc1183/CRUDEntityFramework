using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HospitalPatientManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false),
                    DoctorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Diagnosis = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "CreatedAt", "FullName", "Specialization" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 10, 7, 30, 0, 0, DateTimeKind.Utc), "Dr. Maya Putri", "Internal Medicine" },
                    { 2, new DateTime(2026, 1, 10, 7, 35, 0, 0, DateTimeKind.Utc), "Dr. Raka Wijaya", "Pediatrics" },
                    { 3, new DateTime(2026, 1, 10, 7, 40, 0, 0, DateTimeKind.Utc), "Dr. Nanda Lestari", "Cardiology" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "CreatedAt", "DateOfBirth", "FullName", "Gender", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "Jakarta", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1990, 5, 12), "Budi Santoso", "Male", "081234567890" },
                    { 2, "Bandung", new DateTime(2026, 1, 10, 8, 5, 0, 0, DateTimeKind.Utc), new DateOnly(1988, 11, 3), "Siti Rahma", "Female", "081298765432" },
                    { 3, "Surabaya", new DateTime(2026, 1, 10, 8, 10, 0, 0, DateTimeKind.Utc), new DateOnly(2001, 2, 18), "Andi Pratama", "Male", "081322233344" }
                });

            migrationBuilder.InsertData(
                table: "MedicalRecords",
                columns: new[] { "Id", "Diagnosis", "DoctorId", "PatientId", "VisitDate" },
                values: new object[,]
                {
                    { 1, "Seasonal flu", 1, 1, new DateTime(2026, 1, 11, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Hypertension", 3, 2, new DateTime(2026, 1, 12, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Mild asthma", 2, 3, new DateTime(2026, 1, 13, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "Chest pain follow-up", 3, 1, new DateTime(2026, 1, 14, 13, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "Gastritis", 1, 2, new DateTime(2026, 1, 15, 15, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
