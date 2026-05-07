using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_projekt.Migrations
{
    /// <inheritdoc />
    public partial class FixZookeeperForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enclosures_Zookeepers_ZookeeperIdId",
                table: "Enclosures");

            migrationBuilder.RenameColumn(
                name: "ZookeeperIdId",
                table: "Enclosures",
                newName: "ZookeeperId");

            migrationBuilder.RenameIndex(
                name: "IX_Enclosures_ZookeeperIdId",
                table: "Enclosures",
                newName: "IX_Enclosures_ZookeeperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enclosures_Zookeepers_ZookeeperId",
                table: "Enclosures",
                column: "ZookeeperId",
                principalTable: "Zookeepers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enclosures_Zookeepers_ZookeeperId",
                table: "Enclosures");

            migrationBuilder.RenameColumn(
                name: "ZookeeperId",
                table: "Enclosures",
                newName: "ZookeeperIdId");

            migrationBuilder.RenameIndex(
                name: "IX_Enclosures_ZookeeperId",
                table: "Enclosures",
                newName: "IX_Enclosures_ZookeeperIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enclosures_Zookeepers_ZookeeperIdId",
                table: "Enclosures",
                column: "ZookeeperIdId",
                principalTable: "Zookeepers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
