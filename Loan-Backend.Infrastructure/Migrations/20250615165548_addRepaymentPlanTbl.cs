using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loan_Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRepaymentPlanTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RepaymentId",
                table: "PaymentLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPerInstallment",
                table: "CustomerLoans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfInstallments",
                table: "CustomerLoans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerLoanRepaymentPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    DatePaid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLoanRepaymentPlan", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerLoanRepaymentPlan");

            migrationBuilder.DropColumn(
                name: "RepaymentId",
                table: "PaymentLogs");

            migrationBuilder.DropColumn(
                name: "AmountPerInstallment",
                table: "CustomerLoans");

            migrationBuilder.DropColumn(
                name: "NumberOfInstallments",
                table: "CustomerLoans");
        }
    }
}
