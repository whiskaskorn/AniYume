using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;

namespace Data;

public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}