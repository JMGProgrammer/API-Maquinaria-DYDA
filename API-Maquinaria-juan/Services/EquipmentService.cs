using API_Maquinaria_juan.DTOs;
using API_Maquinaria_juan.Interfaces;
using API_Maquinaria_juan.Models;

namespace API_Maquinaria_juan.Services
{
    // REGLAS DE NEGOCIO de la capa equipment:
    //   1. Normalizar Code a mayúsculas (para que "abc-1" y "ABC-1" sean el mismo).
    //   2. No permitir Codes duplicados (lo avisa lanzando excepción → controller
    //      la traduce a 409 Conflict).
    //   3. Generar el Id secuencial "eq-N" al crear.
    //
    // Esta clase es la ÚNICA que entiende tanto del modelo Equipment (por los
    // mapeadores de abajo) como de los DTOs. El controller recibe/entrega DTOs
    // y el repository solo mueve entidades.
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

        // La EXCEPTION como señal de "regla de negocio rota":
        // el service no sabe qué status code HTTP mostrar, así que no puede
        // devolver 409 él mismo. Lanza InvalidOperationException ("la operación
        // no es válida en este estado") y DELEGA al controller atraparla y
        // traducirla a Conflict(). InvalidOperationException es la excepción
        // estándar de .NET para "el estado actual no permite esto"
        // (un Code duplicado es exactamente eso).
        public async Task<EquipmentReadDTO> CreateAsync(EquipmentCreateDTO input)
        {
            var code = input.Code.Trim().ToUpperInvariant();

            if (await _repository.ExistsByCodeAsync(code))
            {
                throw new InvalidOperationException("El código ya existe");
            }

            // Generación del Id secuencial "eq-N":
            // baja TODOS los Ids existentes, extrae el número (Substring(3) = lo
            // que sigue a "eq-", ej: de "eq-12" queda "12"), se queda con el mayor
            // y le suma 1. DefaultIfEmpty(0) evita la excepción de Max() cuando
            // la tabla está vacía (Max de una lista vacía lanza InvalidOperationException!).
            var ids = await _repository.GetAllIdsAsync();

            var numeros = ids.Select(id => int.Parse(id.Substring(3)));

            var siguiente = numeros.DefaultIfEmpty(0).Max() + 1;

            var equipo = ToEntity(input);

            // El Id se asigna FUERA de ToEntity a propósito: el Id no viene del
            // DTO (el cliente no lo elige), es una regla del alta de este service.
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

            // ¿Por qué usar ToEntity en un update si ya tengo la entidad cargada?
            // 1) Reutilizo la normalización de Code que hace ToEntity (no la repito acá).
            // 2) Se genera una copia "limpia" con SOLO los campos editables; los
            //    paso a la entidad traída de la DB campo a campo, porque el Id
            //    de esa entidad NO se toca jamás en un update.
            var cambios = ToEntity(input);

            // Mismo check que en Create, pero con Excluding: el Code duplicado
            // tiene que ser de OTRO equipo; el propio equipo puede conservar su
            // mismo código al re-editarlo.
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

        // ===== MAPEADORES =====
        //
        // Son las funciones "puente" entre el mundo exterior (DTOs) y la
        // base de datos (entidad Equipment). Toda conversión DTO ↔ entidad de
        // este modelo empieza y termina acá.
        //
        // ¿Por qué private static?
        // - static: son funciones puras, no tocan _repository ni guardan estado;
        //   no necesitan una instancia del service para correr.
        // - private: nadie afuera del service las necesita; el controller recibe
        //   DTOs ya armados y listo.
        //
        // ToEntity: ENTRADA. Recibe EquipmentInputDTO (la CLASE BASE, por eso el
        // mismo método sirve para EquipmentCreateDTO y EquipmentUpdateDTO, que
        // heredan de ella). Aplica el Trim + mayúsculas de Code en un solo lugar.
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

        // ToReadDto: SALIDA. Entidad completa de la DB → DTO que se envía al cliente.
        // Este es el ÚNICO lugar donde el Id se transfiere al DTO: la entidad
        // siempre lo tiene, pero solo se lo entrega al cliente al leer/guardar.
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
