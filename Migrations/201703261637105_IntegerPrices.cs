using System;
using System.Data.Entity.Migrations;

namespace CatsCloset.Migrations {
    public partial class IntegerPrices : DbMigration {
        public override void Up() {
			Sql("UPDATE Customers SET Balance = Balance * 100");
			Sql("UPDATE Histories SET BalanceChange = BalanceChange * 100");
			Sql("UPDATE Products SET Price = Price * 100");
            AlterColumn("dbo.Customers", "Balance", c => c.Int(nullable: false));
            AlterColumn("dbo.Histories", "BalanceChange", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "Price", c => c.Int(nullable: false));
        }
        
		public override void Down() {
			Sql("UPDATE Customers SET Balance = Balance / 100");
			Sql("UPDATE Histories SET BalanceChange = BalanceChange / 100");
			Sql("UPDATE Products SET Price = Price / 100");
            AlterColumn("dbo.Products", "Price", c => c.Double(nullable: false));
            AlterColumn("dbo.Histories", "BalanceChange", c => c.Double(nullable: false));
            AlterColumn("dbo.Customers", "Balance", c => c.Double(nullable: false));
        }
    }
}
