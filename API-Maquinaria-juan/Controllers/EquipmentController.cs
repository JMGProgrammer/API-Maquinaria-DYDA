using API_Maquinaria_juan.DTOs;
using API_Maquinaria_juan.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_Maquinaria_juan.Controllers
{
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

        [HttpPost]
        public async Task<IActionResult> Post(EquipmentCreateDTO input)
        {
            try
            {
                var equipo = await _service.CreateAsync(input);

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
                    return NotFound("Equipamiento inexistente");
                }

                return Ok(equipo);
            }
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
