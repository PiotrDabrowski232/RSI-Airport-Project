namespace Airport.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AirplaneTickets",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PassengerName = c.String(),
                        PassengerSurname = c.String(),
                        FlightID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flights", t => t.FlightID, cascadeDelete: true)
                .Index(t => t.FlightID);
            
            CreateTable(
                "dbo.Flights",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FlightFrom = c.String(),
                        FlightTo = c.String(),
                        DepartureDate = c.DateTime(nullable: false),
                        ArrivalDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AirplaneTickets", "FlightID", "dbo.Flights");
            DropIndex("dbo.AirplaneTickets", new[] { "FlightID" });
            DropTable("dbo.Flights");
            DropTable("dbo.AirplaneTickets");
        }
    }
}
