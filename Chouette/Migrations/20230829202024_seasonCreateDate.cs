using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chouette.Migrations
{
    /// <inheritdoc />
    public partial class seasonCreateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Seasons",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Seasons");
        }
    }
}
