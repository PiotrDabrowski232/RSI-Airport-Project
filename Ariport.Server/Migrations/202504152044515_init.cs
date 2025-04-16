namespace Ariport.Server.Migrations
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
                        PassengerID = c.Guid(nullable: false),
                        FlightID = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Flights", t => t.FlightID, cascadeDelete: true)
                .ForeignKey("dbo.Passengers", t => t.PassengerID, cascadeDelete: true)
                .Index(t => t.PassengerID)
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AirplaneTickets", "PassengerID", "dbo.Passengers");
            DropForeignKey("dbo.AirplaneTickets", "FlightID", "dbo.Flights");
            DropIndex("dbo.AirplaneTickets", new[] { "FlightID" });
            DropIndex("dbo.AirplaneTickets", new[] { "PassengerID" });
            DropTable("dbo.Passengers");
            DropTable("dbo.Flights");
            DropTable("dbo.AirplaneTickets");
        }
    }
}
