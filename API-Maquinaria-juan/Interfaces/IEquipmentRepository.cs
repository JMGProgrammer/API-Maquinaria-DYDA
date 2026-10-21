using API_Maquinaria_juan.Models;

namespace API_Maquinaria_juan.Interfaces
{
    // CONTRATO de acceso a datos: solo CRUD contra la DB, CERO reglas de negocio.
    //
    // La división de responsabilidades queda así:
    //   Repository = "cómo se guardan/leen los datos"      (SQL / LINQ / EF Core)
    //   Service    = "qué reglas se cumplen con esos datos" (duplicados, Ids secuenciales)
    //   Controller = "cómo se entra por HTTP"               (rutas, status codes)
    //
    // Si mañana el storage cambia (postgre, archivos, API externa), se reescribe
    // esta implementación y el Service ni se entera: solo pide la interface.
    // Nota: ExistsByCodeExcludingAsync existe por una razón concreta: en el UPDATE
    // hay que validar que el Code nuevo no lo use OTRO equipo (el propio está
    // bien, porque él mismo se está editando) → "existe el código, excluyendo este Id".
    public interface IEquipmentRepository
    {
        Task<List<Equipment>> GetAllAsync();

        Task<Equipment?> GetByIdAsync(string id);

        Task<bool> ExistsByCodeAsync(string code);

        Task<bool> ExistsByCodeExcludingAsync(string code, string id);

        Task<List<string>> GetAllIdsAsync();

        Task<Equipment> AddAsync(Equipment equipment);

        Task UpdateAsync(Equipment equipment);

        Task<bool> DeleteAsync(string id);
    }
}
