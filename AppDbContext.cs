public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public virtual DbSet<Usuario> Usuarios { get; set; }

}
