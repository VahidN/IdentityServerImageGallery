using Microsoft.EntityFrameworkCore.Migrations;

namespace DNT.IDP.DataLayer.Migrations
{
    public partial class V2018_09_19_1255 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserLogins_SubjectId",
                table: "UserLogins");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_SubjectId_LoginProvider",
                table: "UserLogins",
                columns: new[] { "SubjectId", "LoginProvider" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserLogins_SubjectId_LoginProvider",
                table: "UserLogins");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_SubjectId",
                table: "UserLogins",
                column: "SubjectId");
        }
    }
}
