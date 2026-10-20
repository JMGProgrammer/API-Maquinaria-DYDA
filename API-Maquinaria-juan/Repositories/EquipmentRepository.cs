using API_Maquinaria_juan.Data;
using API_Maquinaria_juan.Interfaces;
using API_Maquinaria_juan.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Maquinaria_juan.Repositories
{
    // Acceso a datos con EF Core. Aquí vive TODO el contacto con el DbContext;
    // cada método de mutación hace su propio SaveChangesAsync.
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _context;

        public EquipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Equipment>> GetAllAsync()
        {
            return _context.Equipments.ToListAsync();
        }

        public async Task<Equipment?> GetByIdAsync(string id)
        {
            return await _context.Equipments.FindAsync(id);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Equipments.AnyAsync(e => e.Code == code);
        }

        public async Task<bool> ExistsByCodeExcludingAsync(string code, string id)
        {
            return await _context.Equipments
                .AnyAsync(e => e.Code == code && e.Id != id);
        }

        public Task<List<string>> GetAllIdsAsync()
        {
            return _context.Equipments
                .Select(e => e.Id)
                .ToListAsync();
        }

        public async Task<Equipment> AddAsync(Equipment equipment)
        {
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();
            return equipment;
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            _context.Equipments.Update(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return false;
            }

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
