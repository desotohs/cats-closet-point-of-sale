using System;
using System.Data.Entity.Migrations;

namespace CatsCloset.Migrations {
    public partial class InitialVersion : DbMigration {
        public override void Up() {
            CreateTable(
                "dbo.CustomerProperties",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        PropertyId = c.Int(nullable: false),
                        Value = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.CustomProperties", t => t.PropertyId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.PropertyId);
            CreateTable(
                "dbo.Customers",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Balance = c.Double(nullable: false),
                        ImageId = c.Int(nullable: false),
                        Barcode = c.String(unicode: false),
                        Pin = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.ImageId);
            CreateTable(
                "dbo.Images",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            CreateTable(
                "dbo.Histories",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false, precision: 0),
                        UserId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        BalanceChange = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CustomerId);
            CreateTable(
                "dbo.HistoryPurchases",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Histories", t => t.HistoryId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.HistoryId);
            CreateTable(
                "dbo.Products",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        ImageId = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        Category = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.ImageId);
            CreateTable(
                "dbo.Users",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(unicode: false),
                        PasswordHash = c.Binary(),
                        Salt = c.Binary(),
                        Token = c.String(unicode: false),
                        StoreAccess = c.Boolean(nullable: false),
                        OfficeAccess = c.Boolean(nullable: false),
                        SettingsAccess = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            CreateTable(
                "dbo.CustomProperties",
                c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            CreateTable(
                "dbo.Options",
                c => new {
                        Key = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Value = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Key);
            CreateTable(
                "dbo.SessionMessages",
                c => new {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Content = c.String(unicode: false),
                        LastUpdate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down() {
            DropForeignKey("dbo.CustomerProperties", "PropertyId", "dbo.CustomProperties");
            DropForeignKey("dbo.CustomerProperties", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Histories", "UserId", "dbo.Users");
            DropForeignKey("dbo.HistoryPurchases", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "ImageId", "dbo.Images");
            DropForeignKey("dbo.HistoryPurchases", "HistoryId", "dbo.Histories");
            DropForeignKey("dbo.Histories", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Customers", "ImageId", "dbo.Images");
            DropIndex("dbo.Products", new[] { "ImageId" });
            DropIndex("dbo.HistoryPurchases", new[] { "HistoryId" });
            DropIndex("dbo.HistoryPurchases", new[] { "ProductId" });
            DropIndex("dbo.Histories", new[] { "CustomerId" });
            DropIndex("dbo.Histories", new[] { "UserId" });
            DropIndex("dbo.Customers", new[] { "ImageId" });
            DropIndex("dbo.CustomerProperties", new[] { "PropertyId" });
            DropIndex("dbo.CustomerProperties", new[] { "CustomerId" });
            DropTable("dbo.SessionMessages");
            DropTable("dbo.Options");
            DropTable("dbo.CustomProperties");
            DropTable("dbo.Users");
            DropTable("dbo.Products");
            DropTable("dbo.HistoryPurchases");
            DropTable("dbo.Histories");
            DropTable("dbo.Images");
            DropTable("dbo.Customers");
            DropTable("dbo.CustomerProperties");
        }
    }
}
