using API_Maquinaria_juan.DTOs;
using API_Maquinaria_juan.Interfaces;
using API_Maquinaria_juan.Models;

namespace API_Maquinaria_juan.Services
{
    // Reglas de negocio: normalizar Code a mayúsculas, rechazar duplicados (409)
    // y generar el Id secuencial "eq-N". El controller ya no conoce el DbContext.
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _repository;

        public EquipmentService(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Equipment>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Equipment?> GetByIdAsync(string id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task<Equipment> CreateAsync(EquipmentCreateDTO input)
        {
            var code = input.Code.Trim().ToUpperInvariant();

            if (await _repository.ExistsByCodeAsync(code))
            {
                throw new InvalidOperationException("El código ya existe");
            }

            var ids = await _repository.GetAllIdsAsync();

            var numeros = ids.Select(id => int.Parse(id.Substring(3)));

            var siguiente = numeros.DefaultIfEmpty(0).Max() + 1;

            var equipo = new Equipment
            {
                Id = "eq-" + siguiente,
                Name = input.Name.Trim(),
                Code = code,
                Location = input.Location.Trim(),
                Status = input.Status
            };

            return await _repository.AddAsync(equipo);
        }

        public async Task<Equipment?> UpdateAsync(string id, EquipmentUpdateDTO input)
        {
            var equipo = await _repository.GetByIdAsync(id);

            if (equipo == null)
            {
                return null; // el controller lo traduce a 404
            }

            var code = input.Code.Trim().ToUpper();

            if (await _repository.ExistsByCodeExcludingAsync(code, id))
            {
                throw new InvalidOperationException("El código ya existe");
            }

            equipo.Name = input.Name.Trim();
            equipo.Code = code;
            equipo.Location = input.Location.Trim();
            equipo.Status = input.Status;

            await _repository.UpdateAsync(equipo);

            return equipo;
        }

        public Task<bool> DeleteAsync(string id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}
