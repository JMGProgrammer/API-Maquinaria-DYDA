using API_Maquinaria_juan.Models;

namespace API_Maquinaria_juan.Interfaces
{
    // Contrato de acceso a datos (solo CRUD contra la DB, sin reglas de negocio).
    public interface IEquipmentRepository
    {
        Task<List<Equipment>> GetAllAsync();

        Task<Equipment?> GetByIdAsync(string id);

        Task<bool> ExistsByCodeAsync(string code);

        Task<bool> ExistsByCodeExcludingAsync(string code, string id);

        Task<List<string>> GetAllIdsAsync();

        Task<Equipment> AddAsync(Equipment equipment);

        Task UpdateAsync(Equipment equipment);

        Task<bool> DeleteAsync(string id);
    }
}
