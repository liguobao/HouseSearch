using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using HouseMap.Dao.DBEntity;

namespace HouseMap.Dao
{
    public class HouseDataContext : DbContext
    {
        public HouseDataContext(DbContextOptions<HouseDataContext> options)
            : base(options)
        {
        }
        public DbSet<UserHouse> UserHouses { get; set; }


        public DbSet<DbConfig> Configs { get; set; }


    }
}
