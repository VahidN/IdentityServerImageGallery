using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNT.IDP.DataLayer.Migrations
{
    public partial class V2018_09_12_1338 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    Username = table.Column<string>(maxLength: 100, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    ClaimType = table.Column<string>(maxLength: 250, nullable: false),
                    ClaimValue = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Users",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 250, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Users",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "SubjectId", "IsActive", "Password", "Username" },
                values: new object[] { "d860efca-22d9-47fd-8249-791ba61b07c7", true, "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", "User 1" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "SubjectId", "IsActive", "Password", "Username" },
                values: new object[] { "b7539694-97e7-4dfe-84da-b4256e1ff5c7", true, "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", "User 2" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[,]
                {
                    { 1, "given_name", "Vahid", "d860efca-22d9-47fd-8249-791ba61b07c7" },
                    { 2, "family_name", "N", "d860efca-22d9-47fd-8249-791ba61b07c7" },
                    { 3, "address", "Main Road 1", "d860efca-22d9-47fd-8249-791ba61b07c7" },
                    { 4, "role", "PayingUser", "d860efca-22d9-47fd-8249-791ba61b07c7" },
                    { 5, "subscriptionlevel", "PayingUser", "d860efca-22d9-47fd-8249-791ba61b07c7" },
                    { 6, "country", "ir", "d860efca-22d9-47fd-8249-791ba61b07c7" },
                    { 7, "given_name", "User 2", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                    { 8, "family_name", "Test", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                    { 9, "address", "Big Street 2", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                    { 10, "role", "FreeUser", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                    { 11, "subscriptionlevel", "FreeUser", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                    { 12, "country", "be", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_SubjectId",
                table: "UserClaims",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_SubjectId",
                table: "UserLogins",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
