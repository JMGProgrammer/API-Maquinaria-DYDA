using System.ComponentModel.DataAnnotations;

namespace API_Maquinaria_juan.DTOs
{
    // Body del PUT (actualización completa). Hoy es gemelo del CreateDTO;
    // a futuro aquí van las reglas propias del update (ej: campos no modificables).
    public class EquipmentUpdateDTO
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Operativo";
    }
}
