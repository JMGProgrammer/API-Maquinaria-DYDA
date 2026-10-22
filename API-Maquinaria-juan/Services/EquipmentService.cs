using API_Maquinaria_juan.DTOs;
using API_Maquinaria_juan.Interfaces;
using API_Maquinaria_juan.Models;

namespace API_Maquinaria_juan.Services
{
    // REGLAS DE NEGOCIO de la capa equipment:
    //   1. Normalizar Code a mayúsculas (para que "abc-1" y "ABC-1" sean el mismo).
    //   2. Validar los datos de entrada con Validar() → ante cualquier falla lanza
    //      ArgumentException, que el controller atrapa y traduce a 400 BadRequest.
    //   3. Generar el Id secuencial "eq-N" al crear.
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

        public async Task<EquipmentReadDTO> CreateAsync(EquipmentCreateDTO input)
        {
            // Todo lo que puede fallar por dato inválido sale a la superficie acá:
            // si Validar() lanza ArgumentException, el flujo corta ANTES de tocar la DB.
            Validar(input);

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

            // Validar cubre los campos de entrada; si algo no cumple rompe acá.
            Validar(input);

            // ¿Por qué usar ToEntity en un update si ya tengo la entidad cargada?
            // 1) Reutilizo la normalización de Code que hace ToEntity (no la repito acá).
            // 2) Se genera una copia "limpia" con SOLO los campos editables; los
            //    paso a la entidad traída de la DB campo a campo, porque el Id
            //    de esa entidad NO se toca jamás en un update.
            var cambios = ToEntity(input);

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

        // ===== VALIDACIÓN CENTRALIZADA =====
        //
        // Toda regla de "dato inválido" vive acá, en un solo lugar. Si algo no
        // cumple, lanza ArgumentException con su mensaje de causa puntual; el
        // controller atrapa la excepción y la convierte en 400 BadRequest(msg).
        //
        // ¿Por qué ArgumentException? Es la excepción estándar de .NET para
        // "me pasaron un argumento/valor que no sirve" (los campos del body son
        // justamente los argumentos de esta operación).
        //
        // ¿Por qué síncrono? Solo mira los valores del DTO en memoria: no toca
        // la DB ni nada lento → no se necesita async.
        //
        // ¿No están duplicados los [Required] del DTO? Es una validación en doble capa:
        // los DataAnnotations cortan ANTEMANO con un 400 genérico de ASP.NET;
        // Validar() es la segunda muralla (y la que pone mensajes en español con
        // dominio de negocio). Servicio robusto = desconfiar de los datos aunque
        // "ya fueran validados" en otra capa.
        private void Validar(EquipmentInputDTO input)
        {
            // --- Name ---
            // IsNullOrWhiteSpace cubre campos "" o con solo espacios, que [Required]
            // solos no filtran bien.
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new ArgumentException("El nombre es obligatorio");
            }

            if (input.Name.Trim().Length > 100)
            {
                throw new ArgumentException("El nombre no puede superar los 100 caracteres");
            }

            // --- Code ---
            if (string.IsNullOrWhiteSpace(input.Code))
            {
                throw new ArgumentException("El código es obligatorio");
            }

            if (input.Code.Trim().Length > 30)
            {
                throw new ArgumentException("El código no puede superar los 30 caracteres");
            }

            // --- Location / Status ---
            if (string.IsNullOrWhiteSpace(input.Location))
            {
                throw new ArgumentException("La ubicación es obligatoria");
            }

            if (string.IsNullOrWhiteSpace(input.Status))
            {
                throw new ArgumentException("El estado es obligatorio");
            }
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
