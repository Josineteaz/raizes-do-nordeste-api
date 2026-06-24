using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.API.Domain.Entities;
using RaizesDoNordeste.API.Domain.Interfaces;

namespace RaizesDoNordeste.API.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
        }

        public async Task<IEnumerable<Usuario>> ObterTodosAsync()
        {
            return await _context.Usuarios.Where(u => u.Ativo).ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> ObterTodosPaginadoAsync(int page, int limit)
        {
            return await _context.Usuarios
                .Where(u => u.Ativo) 
                .AsNoTracking() 
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            usuario.Ativo = true;
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }


        public async Task DesativarAsync(int id)
        {
            var usuario = await ObterPorIdAsync(id);
            if (usuario != null)
            {
                usuario.Ativo = false;
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}