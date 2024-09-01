[ApiController]
[Route("api/auth")]
public class AuthController(UserManager<IdentityUser> userManager,
                      SignInManager<IdentityUser> signInManager,
                      RoleManager<IdentityRole> roleManager,
                      IConfiguration configuration,
                      AppDbContext db) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email.Split("@")[0], Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var roleUser = roleManager.FindByNameAsync("USER");
                    if (roleUser != null)
                    {
                        await roleManager.CreateAsync(new IdentityRole { Name = "USER" });
                    }

                    await userManager.AddToRoleAsync(user, "USER");

                    var usuario = new Usuario()
                    {
                        Nombres = model.Nombres,
                        Apellidos = model.Apellidos,
                        Celular = model.Celular,
                        Direccion = model.Direccion,
                        
                    };
                    await db.Usuarios.AddAsync(usuario);
                    await db.SaveChangesAsync();
                    return Ok(new { StatusCode = 200, Mensaje = "User registered successfully." });
                }

                return BadRequest(new { StatusCode = 400, Mensaje = result.Errors });
            }

            return BadRequest(new { StatusCode = 400, Mensaje = "Invalid Data." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { StatusCode = 500, Mensaje = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await userManager.CheckPasswordAsync(user, model.Password);
                    if (result)
                    {
                        var token = await GenerateJwtToken(user!);
                        return Ok(new { StatusCode = 200, Token = token });
                    }
                    return Unauthorized(new { StatusCode = 401, Mensaje = "Contraseña invalida." });
                }

                return BadRequest(new { StatusCode = 400, Mensaje = "Usuario no encontrado." });
            }

            return BadRequest(new { StatusCode = 400, Mensaje = "Invalid Data." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { StatusCode = 500, Mensaje = ex.Message });
        }
    }

    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new ("Id", user.Id)
        };

        var roles = await userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(x => new Claim("Role", x));
        claims.AddRange(roleClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
