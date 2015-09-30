namespace thisistracer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTableName1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TRC_Roles", newName: "TRC_Role");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.TRC_Role", newName: "TRC_Roles");
        }
    }
}
