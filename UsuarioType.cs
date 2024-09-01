//types para graphql
public class UsuarioType : ObjectType<Usuario>
{
    protected override void Configure(IObjectTypeDescriptor<Usuario> descriptor)
    {
        descriptor.Description("Representa un producto en el catálogo.");
        // Configura los campos y sus resolvers si es necesario
        descriptor.Field(p => p.Id).Description("El ID del usuario.");
        descriptor.Field(p => p.Nombres).Description("El nombre del usuario.");
        descriptor.Field(p => p.Apellidos).Description("El apellido del usuario.");
        descriptor.Field(p => p.Celular).Description("El cel del Usuario");
        descriptor.Field(p => p.Direccion).Description("Direc del usuario.");
    }
}
