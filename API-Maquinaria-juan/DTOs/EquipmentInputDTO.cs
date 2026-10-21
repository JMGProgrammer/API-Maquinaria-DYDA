using System.ComponentModel.DataAnnotations;

namespace API_Maquinaria_juan.DTOs
{
    // DTO base con los campos que el cliente envía (Name, Code, Location, Status).
    // CreateDTO y UpdateDTO heredan de aquí para no repetir las validaciones;
    // es abstract porque nunca se recibe por sí solo en un endpoint.
    public abstract class EquipmentInputDTO
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
