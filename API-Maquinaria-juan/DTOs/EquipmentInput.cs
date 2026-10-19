using System.ComponentModel.DataAnnotations;

namespace API_Maquinaria_juan.DTOs
{
    public class EquipmentInput
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
