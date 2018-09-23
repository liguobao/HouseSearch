using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HouseMapAPI.DBEntity
{
    public class HouseDataContext : DbContext
    {
        public HouseDataContext(DbContextOptions<HouseDataContext> options)
            : base(options)
        {
        }
        public DbSet<UserHouse> UserHouses { get; set; }

    }
}
