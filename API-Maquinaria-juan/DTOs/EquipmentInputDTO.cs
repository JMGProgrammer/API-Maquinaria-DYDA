using System.ComponentModel.DataAnnotations;

namespace API_Maquinaria_juan.DTOs
{
    // DTO base con TODOS los campos que el cliente envía (Name, Code, Location, Status).
    //
    // ¿Por qué NO hay Id acá?
    // El Id lo genera la API ("eq-N") al momento del alta. Si el cliente pudiera
    // mandarlo en el body, podría forzar el Id que quiera (ej: "eq-999") y
    // romper la numeración secuencial → regla: DE ENTRADA solo entra lo que
    // el usuario puede decidir.
    //
    // ¿Por qué heredan de acá (CreateDTO / UpdateDTO)?
    // - Las validaciones de abajo se escriben UNA sola vez y valen para ambos.
    // - abstract = esta clase no se puede recibir "suelta" en ningún endpoint;
    //   siempre se entra por una de sus hijas.
    // - Mañana UpdateDTO puede agregar o cambiar validaciones propias
    //   sin afectar al Create (gracias a que se separaron).
    //
    // ¿Cómo se aplican las validaciones? Con [ApiController], ASP.NET valida
    // los atributos ANTES de entrar al método del controller:
    // falla algo → responde 400 automáticamente, ni siquiera llegamos al código.
    public abstract class EquipmentInputDTO
    {
        // Required + StringLength → obligatorio y máximo 100 caracteres.
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Code { get; set; } = string.Empty;

        [Required] // Location no tiene StringLength (textos largos permitidos).
        public string Location { get; set; } = string.Empty;

        // Da igual no mandarlo: si Status no llega en el JSON, el binding usa
        // este default "Operativo" (por eso es útil el valor inicial en C#).
        [Required]
        public string Status { get; set; } = "Operativo";
    }
}
