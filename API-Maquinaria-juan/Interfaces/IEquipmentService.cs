using API_Maquinaria_juan.DTOs;

namespace API_Maquinaria_juan.Interfaces
{
    // Contrato de la capa de negocio: solo entiende DTOs (el controller no
    // conoce el modelo Equipment) y lanza InvalidOperationException cuando una
    // regla de negocio no se cumple.

    public interface IEquipmentService
    {
        Task<List<EquipmentReadDTO>> GetAllAsync();

        Task<EquipmentReadDTO?> GetByIdAsync(string id);

        Task<EquipmentReadDTO> CreateAsync(EquipmentCreateDTO input);

        Task<EquipmentReadDTO?> UpdateAsync(string id, EquipmentUpdateDTO input);

        Task<bool> DeleteAsync(string id);
    }
}
