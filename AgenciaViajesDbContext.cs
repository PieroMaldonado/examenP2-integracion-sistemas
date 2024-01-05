using examenP2;
using Microsoft.EntityFrameworkCore;


public class AgenciaViajesDbContext : DbContext
{
    public DbSet<UsuarioModel> Usuario { get; set; }
    // Otras DbSet según sea necesario

    public AgenciaViajesDbContext(DbContextOptions<AgenciaViajesDbContext> options) : base(options)
    {
    }

    // Otras DbSet aquí...

    public DbSet<UsuarioGeoreferencia> UsuarioGeoreferencia { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aquí puedes agregar configuraciones adicionales para tus entidades si es necesario
    }
}