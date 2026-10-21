using API_Maquinaria_juan.DTOs;

namespace API_Maquinaria_juan.Interfaces
{
    // CONTRATO de la capa de negocio.
    //
    // Una interface solo declara QUÉ métodos existen, sin implementarlos
    // (cuerpo vacío / sin llaves). Quien la implemente ("class X : IEquipmentService")
    // PROMETE tener todos esos métodos; si falta uno, no compila → el compilador
    // te obliga a cumplir el contrato.
    //
    // ¿Para qué realmente sirve acá?
    //
    // 1. Inyección de dependencias: el controller pide "IEquipmentService" en su
    //    constructor (nunca la clase concreta). Program.cs hace el cableado:
    //    builder.Services.AddScoped<IEquipmentService, EquipmentService>();
    //    .NET crea y destruye una instancia por cada request HTTP (scoped).
    //
    // 2. Testeabilidad: en un test se puede pasar un repositorio falso al service
    //    y probar la lógica sin tocar la base de datos.
    //
    // 3. Desacoplar capas: el controller no sabe que existe EquipmentService ni
    //    Equipment (modelo). Solo domina DTOs (ver: de acá ya no sale el using
    //    a API_Maquinaria_juan.Models).
    //
    // Detalles de las firmas:
    // - Task<T> es lo que expresa la asincronía: el repository hace I/O contra
    //   MySQL y, mientras espera la respuesta, el hilo queda libre.
    // - "T?" significa "puede no existir" (devuelve null): el controller traduce
    //   ese null a 404 NotFound. DeleteAsync es bool y no T? → false = no estaba.
    public interface IEquipmentService
    {
        Task<List<EquipmentReadDTO>> GetAllAsync();

        Task<EquipmentReadDTO?> GetByIdAsync(string id);

        Task<EquipmentReadDTO> CreateAsync(EquipmentCreateDTO input);

        Task<EquipmentReadDTO?> UpdateAsync(string id, EquipmentUpdateDTO input);

        Task<bool> DeleteAsync(string id);
    }
}
