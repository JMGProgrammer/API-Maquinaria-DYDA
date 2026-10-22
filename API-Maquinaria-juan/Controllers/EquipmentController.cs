using API_Maquinaria_juan.DTOs;
using API_Maquinaria_juan.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_Maquinaria_juan.Controllers
{
    // CAPA WEB: rutas + código HTTP, nada de lógica de negocio.
    // [ApiController] además valida automáticamente los DTOs ([Required] etc.):
    // si falla la validación responde 400 sin ejecutar ni una línea del método.
    // [Route("api/[controller]")] → "Equipment" menos "Controller" → /api/equipment
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _service;

        public EquipmentController(IEquipmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var equipments = await _service.GetAllAsync();
            return Ok(equipments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentById(string id)
        {
            var equipment = await _service.GetByIdAsync(id);

            if (equipment == null)
            {
                return NotFound(
                    $"No se encontró el equipo con ID {id}."
                );
            }

            return Ok(equipment);
        }

        // ===== TRY & CATCH =====
        //
        // Reparto de trabajo entre capas:
        //   - el SERVICE VALIDA con Validar() y LANZA ArgumentException cuando
        //     un dato no cumple una regla (campo vacío, muy largo, etc.),
        //     pero él NO sabe nada de HTTP;
        //   - este CONTROLLER la CAPTA con try/catch y la traduce a un status code
        //     con el mensaje exacto de la causa ("El nombre es obligatorio", etc.).
        //
        // ¿Por qué BadRequest (400)? ArgumentException significa "el argumento
        // (dato de entrada) no es válido" → el problema lo tiene el que envía
        // el body, no el servidor. El service decide cuál regla falló: cada
        // "throw new ArgumentException(...)" tiene su mensaje propio.
        //
        // ¿Por qué catch específico (ArgumentException) y no catch (Exception)?
        // El catch general "traga" también los bugs verdaderos (null reference,
        // fallo de conexión...) disfrazándolos de un 400 común. Con el catch
        // específico, solo atrapamos la excepción que NOSOTROS planeamos throw
        // en el service; lo inesperado sigue estallando.
        [HttpPost]
        public async Task<IActionResult> Post(EquipmentCreateDTO input)
        {
            try
            {
                var equipo = await _service.CreateAsync(input);

                // Created = 201 + cabecera Location apuntando a dónde ver lo nuevo.
                // El service ya devolvió EquipmentReadDTO (mapeado al final de CreateAsync;
                // si algo falló, no llega acá porque Validar() cortó el flujo antes).
                return Created($"/api/equipment/{equipo.Id}", equipo);
            }
            catch (ArgumentException ex)
            {
                // Validar() del service lanzó un dato inválido → 400 con la causa puntual
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, EquipmentUpdateDTO input)
        {
            try
            {
                var equipo = await _service.UpdateAsync(id, input);

                if (equipo == null)
                {
                    // null (la interface lo declara EquipmentReadDTO?) → no estaba
                    // ese Id → 404. El service no conoce HTTP, el controller traduce.
                    return NotFound("Equipamiento inexistente");
                }

                return Ok(equipo);
            }
            // Igual al POST: Validar() del service lanzó ArgumentException → 400
            // con el mensaje de la regla que falló (ej: "El nombre es obligatorio").
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminado = await _service.DeleteAsync(id);

            if (!eliminado)
            {
                return NotFound("Equipamiento inexistente");
            }

            return Ok(new
            {
                message = "Equipamiento eliminado",
                id
            });
        }
    }
}
