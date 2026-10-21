namespace API_Maquinaria_juan.DTOs
{
    // Modelo de salida: lo que la API devuelve al leer un equipo.
    // El Id solo existe en la salida; nunca se recibe por el body.
    public class EquipmentReadDTO
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Status { get; set; } = "Operativo";
    }
}
