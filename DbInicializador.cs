//inicializador de la base de datos
public class DbInicializador(UserManager<IdentityUser> userMana,
    AppDbContext con, RoleManager<IdentityRole> roleMana) : IDbInicializador
{
    public void Inicializar()
    {
        try
        {
            if (con.Database.GetPendingMigrations().Count() > 0)
            {
                con.Database.Migrate(); //ejecutar migraciones pendientes
            }
        }
        catch
        {
            throw;
        }
        //datos iniciales
        if (con.Roles.Any(r => r.Name == "SUPERADMIN")) return;

        //crear roles
        roleMana.CreateAsync(new IdentityRole("SUPERADMIN")).GetAwaiter().GetResult();
        roleMana.CreateAsync(new IdentityRole("USER")).GetAwaiter().GetResult();
        //agrega mas roles segun sea necesario

        //crear usuario
        userMana.CreateAsync(new IdentityUser
        {
            UserName = "nombreusuario",
            Email = "email",
        }, "password").GetAwaiter().GetResult();

        IdentityUser user = userMana.FindByEmailAsync("email").GetAwaiter().GetResult()!;

        userMana.AddToRoleAsync(user, "SUPERADMIN").GetAwaiter().GetResult();

        var usuario = new Usuario()
        {
            Id = user.Id,
            Nombres = "nombres",
            Apellidos = "apellidos",
            Email = "email",
            Celular = "celular",
            Direccion = "direccion"
        };

        con.Usuarios.AddAsync(usuario).GetAwaiter().GetResult();
        con.SaveChangesAsync().GetAwaiter().GetResult();

    }
}
