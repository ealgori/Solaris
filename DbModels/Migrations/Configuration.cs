namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using DbModels.DomainModels.ShClone;
    using DbModels.DataContext;
    using DbModels.DomainModels.Base;

    internal sealed class Configuration : DbMigrationsConfiguration<DbModels.DataContext.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(DbModels.DataContext.Context context)
        {
           
        }

        private void CreateTestWoForPrintout(Context context)
        {
          
        }

    }
}
