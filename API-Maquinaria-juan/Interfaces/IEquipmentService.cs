using API_Maquinaria_juan.DTOs;
using API_Maquinaria_juan.Models;

namespace API_Maquinaria_juan.Interfaces
{
    // Contrato de la capa de negocio: recibe y/o da DTOs y modelos, lanza
    // InvalidOperationException cuando una regla de negocio no se cumple.
    
    public interface IEquipmentService
    {
        Task<List<Equipment>> GetAllAsync();

        Task<Equipment?> GetByIdAsync(string id);

        Task<Equipment> CreateAsync(EquipmentCreateDTO input);

        Task<Equipment?> UpdateAsync(string id, EquipmentUpdateDTO input);

        Task<bool> DeleteAsync(string id);
    }
}
