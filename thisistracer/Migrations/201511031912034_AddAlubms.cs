namespace thisistracer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlubms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        idx = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        AlbumName = c.String(),
                        AlbumCreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.idx);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Albums");
        }
    }
}
