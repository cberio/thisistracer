namespace thisistracer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteAlbumEntity : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Albums");
        }
        
        public override void Down()
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
    }
}
