﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace code.Models
{
    public class DataContext:DbContext
    {
        public DataContext() : base("DataConnectionString") 
        {
            Database.SetInitializer<DataContext>(new DataInitializer());
        }
        public DbSet<Page> Pages { get; set; }
        public DbSet<User> Users { get; set; }
    }
}