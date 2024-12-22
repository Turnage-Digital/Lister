using Lister.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lister.Users.Infrastructure.Sql;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : IdentityDbContext<User>(options);