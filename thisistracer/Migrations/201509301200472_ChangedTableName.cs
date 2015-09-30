namespace thisistracer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AspNetRoles", newName: "TRC_Roles");
            RenameTable(name: "dbo.AspNetUserRoles", newName: "TRC_UserRole");
            RenameTable(name: "dbo.AspNetUsers", newName: "TRC_User");
            RenameTable(name: "dbo.AspNetUserClaims", newName: "TRC_UserClaim");
            RenameTable(name: "dbo.AspNetUserLogins", newName: "TRC_UserLogin");
            RenameColumn(table: "dbo.TRC_Roles", name: "Id", newName: "RoleId");
            RenameColumn(table: "dbo.TRC_User", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.TRC_UserClaim", name: "Id", newName: "UserClaim");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.TRC_UserClaim", name: "UserClaim", newName: "Id");
            RenameColumn(table: "dbo.TRC_User", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.TRC_Roles", name: "RoleId", newName: "Id");
            RenameTable(name: "dbo.TRC_UserLogin", newName: "AspNetUserLogins");
            RenameTable(name: "dbo.TRC_UserClaim", newName: "AspNetUserClaims");
            RenameTable(name: "dbo.TRC_User", newName: "AspNetUsers");
            RenameTable(name: "dbo.TRC_UserRole", newName: "AspNetUserRoles");
            RenameTable(name: "dbo.TRC_Roles", newName: "AspNetRoles");
        }
    }
}
