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

        public async Task<List<EquipmentReadDTO>> GetAllAsync()
        {
            var equipos = await _repository.GetAllAsync();

            return equipos.Select(ToReadDto).ToList();
        }

        public async Task<EquipmentReadDTO?> GetByIdAsync(string id)
        {
            var equipo = await _repository.GetByIdAsync(id);

            // el controller traduce el null a 404
            return equipo == null ? null : ToReadDto(equipo);
        }

        public async Task<EquipmentReadDTO> CreateAsync(EquipmentCreateDTO input)
        {
            var code = input.Code.Trim().ToUpperInvariant();

            if (await _repository.ExistsByCodeAsync(code))
            {
                throw new InvalidOperationException("El código ya existe");
            }

            var ids = await _repository.GetAllIdsAsync();

            var numeros = ids.Select(id => int.Parse(id.Substring(3)));

            var siguiente = numeros.DefaultIfEmpty(0).Max() + 1;

            var equipo = ToEntity(input);
            equipo.Id = "eq-" + siguiente;

            await _repository.AddAsync(equipo);

            return ToReadDto(equipo);
        }

        public async Task<EquipmentReadDTO?> UpdateAsync(string id, EquipmentUpdateDTO input)
        {
            var equipo = await _repository.GetByIdAsync(id);

            if (equipo == null)
            {
                return null; // el controller lo traduce a 404
            }

            var cambios = ToEntity(input);

            if (await _repository.ExistsByCodeExcludingAsync(cambios.Code, id))
            {
                throw new InvalidOperationException("El código ya existe");
            }

            equipo.Name = cambios.Name;
            equipo.Code = cambios.Code;
            equipo.Location = cambios.Location;
            equipo.Status = cambios.Status;

            await _repository.UpdateAsync(equipo);

            return ToReadDto(equipo);
        }

        public Task<bool> DeleteAsync(string id)
        {
            return _repository.DeleteAsync(id);
        }

        // Mapea el DTO de entrada (sirve igual para Create y Update, por heredar
        // de EquipmentInputDTO) a la entidad Equipment. Normaliza Code a mayúsculas.
        private static Equipment ToEntity(EquipmentInputDTO input)
        {
            return new Equipment
            {
                Name = input.Name.Trim(),
                Code = input.Code.Trim().ToUpperInvariant(),
                Location = input.Location.Trim(),
                Status = input.Status
            };
        }

        // Mapea la entidad al DTO de salida; el Id solo se expone al leer.
        private static EquipmentReadDTO ToReadDto(Equipment equipment)
        {
            return new EquipmentReadDTO
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Code = equipment.Code,
                Location = equipment.Location,
                Status = equipment.Status
            };
        }
    }
}
