//querys para graphql
public class Query
{
    [Authorize]
    public IQueryable<Usuario> GetUsuarios([Service] AppDbContext context) => context.Usuarios;
    //public IQueryable<Producto> GetProductos([Service] CatalogoContext context) => context.Productos;
    //public IQueryable<Cliente> GetClientes([Service] CatalogoContext context) => context.Clientes;
    //public IQueryable<Pedido> GetPedidos([Service] CatalogoContext context) => context.Pedidos;
}
