using System.ComponentModel.DataAnnotations;

namespace API_Maquinaria_juan.DTOs
{
    // Body del POST (alta de equipo).
    // Hereda los 4 campos + validaciones de EquipmentInputDTO; el cuerpo queda
    // vacío a propósito: si algún día el alta necesita un campo extra (o una
    // validación distinta), se agrega acá SIN tocar al update.
    // [ApiController] valida los atributos heredados antes de entrar a Post().
    public class EquipmentCreateDTO : EquipmentInputDTO
    {
    }
}
