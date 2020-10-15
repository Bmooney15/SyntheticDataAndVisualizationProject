using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SynthMVC.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SynthMVC.DAL
{
    public class PersonContext : DbContext
    {
        public PersonContext() : base("PersonContext") 
        {

        } //Constructs a new context instance using the given string as the name or connection string for the database to which a connection will be made.

        public DbSet<Person> People { get; set; }
        public DbSet<StateData> States { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
