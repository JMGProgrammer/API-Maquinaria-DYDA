namespace API_Maquinaria_juan.DTOs
{
    // MODELO DE SALIDA: lo que la API devuelve al leer un equipo.
    //
    // Diferencia clave con los DTOs de entrada: este SÍ tiene Id, porque el Id
    // es dato que esta API generó ("eq-N") y el cliente lo necesita para las
    // siguientes operaciones (PUT /api/equipment/{id}, DELETE /api/equipment/{id}).
    //
    // Regla para recordar: DE SALIDA sale todo lo que el cliente necesita ver;
    // DE ENTRADA entra solo lo que el cliente puede decidir.
    public class EquipmentReadDTO
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Status { get; set; } = "Operativo";
    }
}
