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
        //   - el SERVICE lanza la excepción cuando una regla de negocio no se cumple
        //     (ej: "El código ya existe"), pero él NO sabe nada de HTTP;
        //   - este CONTROLLER la CAPTA con try/catch y la traduce a un status code.
        //
        // ¿Por qué catch específico (InvalidOperationException) y no catch (Exception)?
        // El catch general "traga" también los bugs verdaderos (null reference,
        // fallo de conexión...) disfrazándolos de un 409 común. Con el catch
        // específico, solo atrapamos la excepción que NOSOTROS planificamos
        // throw en el service; lo inesperado sigue estallando.
        //
        // 409 Conflict significa "conflicto con el estado actual del recurso",
        // que es exactamente "ya hay un equipo con ese Code".
        [HttpPost]
        public async Task<IActionResult> Post(EquipmentCreateDTO input)
        {
            try
            {
                var equipo = await _service.CreateAsync(input);

                // Created = 201 + cabecera Location apuntando a dónde ver lo nuevo.
                // El service ya devolvió EquipmentReadDTO (mapeado al final de CreateAsync).
                return Created($"/api/equipment/{equipo.Id}", equipo);
            }
            catch (InvalidOperationException ex)
            {
                // regla de negocio rota (código duplicado) → 409
                return Conflict(ex.Message);
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
            // Igual al POST: el service lanzó "Code duplicado" → 409 Conflict.
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
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
