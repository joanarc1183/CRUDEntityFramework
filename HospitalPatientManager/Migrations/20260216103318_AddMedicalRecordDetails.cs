using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalPatientManager.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalRecordDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Treatment",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Notes", "Status", "Treatment" },
                values: new object[] { "Follow-up if fever persists for more than 3 days", "Completed", "Rest, hydration, and paracetamol for fever" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Notes", "Status", "Treatment" },
                values: new object[] { "Patient advised to reduce salt intake and monitor BP at home", "Completed", "Blood pressure medication adjustment" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Notes", "Status", "Treatment" },
                values: new object[] { "Avoid known asthma triggers and carry inhaler", "Completed", "Inhaler prescribed for symptom relief" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Notes", "Status", "Treatment" },
                values: new object[] { "No acute findings; continue monitoring symptoms", "Pending", "ECG and observation" });

            migrationBuilder.UpdateData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Notes", "Status", "Treatment" },
                values: new object[] { "Avoid spicy foods and late-night meals", "Scheduled", "Antacid and dietary modification" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Treatment",
                table: "MedicalRecords");
        }
    }
}

