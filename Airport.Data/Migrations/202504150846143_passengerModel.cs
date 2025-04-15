namespace Airport.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class passengerModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Passengers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Surname = c.String(),
                        Pesel = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AirplaneTickets", "PassengerID", c => c.Guid(nullable: false));
            AddColumn("dbo.AirplaneTickets", "Status", c => c.Int(nullable: false));
            CreateIndex("dbo.AirplaneTickets", "PassengerID");
            AddForeignKey("dbo.AirplaneTickets", "PassengerID", "dbo.Passengers", "Id", cascadeDelete: true);
            DropColumn("dbo.AirplaneTickets", "PassengerName");
            DropColumn("dbo.AirplaneTickets", "PassengerSurname");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AirplaneTickets", "PassengerSurname", c => c.String());
            AddColumn("dbo.AirplaneTickets", "PassengerName", c => c.String());
            DropForeignKey("dbo.AirplaneTickets", "PassengerID", "dbo.Passengers");
            DropIndex("dbo.AirplaneTickets", new[] { "PassengerID" });
            DropColumn("dbo.AirplaneTickets", "Status");
            DropColumn("dbo.AirplaneTickets", "PassengerID");
            DropTable("dbo.Passengers");
        }
    }
}
