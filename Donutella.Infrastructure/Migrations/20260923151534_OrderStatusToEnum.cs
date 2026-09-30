using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donutella.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusTmp",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE Orders SET StatusTmp = CASE Status
                    WHEN 'Pending' THEN 0
                    WHEN 'Shipped' THEN 1
                    WHEN 'Delivered' THEN 2
                    WHEN 'Cancelled' THEN 3
                    ELSE 0 END");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "StatusTmp",
                table: "Orders",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusTmp",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE Orders SET StatusTmp = CASE Status
                    WHEN 0 THEN 'Pending'
                    WHEN 1 THEN 'Shipped'
                    WHEN 2 THEN 'Delivered'
                    WHEN 3 THEN 'Cancelled'
                    ELSE 'Pending' END");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "StatusTmp",
                table: "Orders",
                newName: "Status");
        }
    }
}
