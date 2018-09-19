using Microsoft.EntityFrameworkCore.Migrations;

namespace DNT.IDP.DataLayer.Migrations
{
    public partial class V2018_09_19_0908 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserClaims_SubjectId",
                table: "UserClaims");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_SubjectId_ClaimType",
                table: "UserClaims",
                columns: new[] { "SubjectId", "ClaimType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserClaims_SubjectId_ClaimType",
                table: "UserClaims");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_SubjectId",
                table: "UserClaims",
                column: "SubjectId");
        }
    }
}
