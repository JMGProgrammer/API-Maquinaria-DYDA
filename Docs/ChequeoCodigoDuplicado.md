# Chequeo de código duplicado (funcionalidad retirada)

> **Estado: ELIMINADO del proyecto.** Todavía no se vio con el profe, así que se sacó
> para no cruzar cables con lo que damos en clase. Este documento conserva la idea,
> el código exacto que tenía y cómo volver a activarlo cuando toque.

## ¿Qué hacía?

Cuando alguien creaba o editaba un equipo, la API verificaba en la base de datos que
nadie más tuviera el mismo `Code`. Se normalizaba a mayúsculas antes de comparar para
que `abc-1` y `ABC-1` contaran como el mismo código, y frenaba la operación con un error.

## Funciones que lo implementaban (retiradas)

### 1. En `Interfaces/IEquipmentRepository.cs`

```csharp
// ¿Existe ALGÚN equipo con ese código? (para el CREATE)
Task<bool> ExistsByCodeAsync(string code);

// ¿Existe ese código usado por OTRO equipo distinto de "id"? (para el UPDATE)
Task<bool> ExistsByCodeExcludingAsync(string code, string id);
```

### 2. En `Repositories/EquipmentRepository.cs`

```csharp
// AnyAsync = "¿algún registro cumple esta condición?"
// EF Core lo traduce a SQL:  SELECT COUNT(*) FROM Equipments WHERE Code = @code LIMIT 1
public async Task<bool> ExistsByCodeAsync(string code)
{
    return await _context.Equipments.AnyAsync(e => e.Code == code);
}

// Clave del "Excluding": la condición e.Id != id. Casi siempre el Code duplicado
// será del PROPIO equipo en edición, y ese está permitido (sigue editando su Code).
public async Task<bool> ExistsByCodeExcludingAsync(string code, string id)
{
    return await _context.Equipments
        .AnyAsync(e => e.Code == code && e.Id != id);
}
```

### 3. En `Services/EquipmentService.cs` — la parte que vivía dentro de `Validar()`

```csharp
private async Task Validar(EquipmentInputDTO input, string? idActual = null)
{
    // ...validaciones de campos (las que HOY quedan en Validar())...

    // --- Chequeo duplicado (esto es lo que se retiró) ---
    // CREATE lo llamaba sin idActual; UPDATE lo llamaba pasando el "id".
    var code = input.Code.Trim().ToUpperInvariant();   // misma normalización de ToEntity

    var duplicado = idActual == null
        ? await _repository.ExistsByCodeAsync(code)              // alta: cualquiera que exista molesta
        : await _repository.ExistsByCodeExcludingAsync(code, idActual); // edición: menos el propio

    if (duplicado)
    {
        throw new ArgumentException("El código ya existe");  // 400 hacia el cliente
    }
}
```

El flujo de las llamadas era:

```
CreateAsync(input)            → await Validar(input)         → idActual = null
UpdateAsync(id, input)        → await Validar(input, id)      → idActual = id
```

Y en el `Controller` se atrapaba así (mismo `try/catch` que hoy, solo cambia qué
excepciones llegan):

```csharp
catch (ArgumentException ex)
{
    return BadRequest(ex.Message);   // 400: "El código ya existe"
}
```

## ¿Por qué estaba así diseñado?

- **Ubicación**: la regla "no repetir codes" es NEGOCIO → vive en `Service`, no en el
  controller y no en el repository (que solo sabe preguntar datos, cuál sea la regla).
- **`async`**: el chequeo habla con MySQL → esa era la razón por la que `Validar()`
  era `async Task`. Sin el chequeo, `Validar()` volvió a ser `private void`.
- **Doble invocación con parámetro opcional**: un solo método de validación sirve el
  alta y la edición; el parámetro `idActual = null` cambia el comportamiento.

## Cómo reactivarlo (cuando lo veamos con el profe)

1. Volver a declarar los dos métodos en `IEquipmentRepository` (punto 1) y
   implementarlos en `EquipmentRepository` (punto 2).
2. En `EquipmentService`, cambiar `Validar` a `private async Task Validar(...)`
   con el parámetro `string? idActual = null` y agregar el bloque "Chequeo
   duplicado" del punto 3 al final del método.
3. En `CreateAsync`: `await Validar(input);` y en `UpdateAsync`: `await Validar(input, id);`.
4. No hace falta tocar el controller: el `catch (ArgumentException)` que ya existe
   lo traduce a 400 · `BadRequest` automáticamente.
5. Por último, verificar: dos POST con codes iguales (o con distinta caja:
   `cmp-1` vs `CMP-1`) tienen que responder el 400 con "El código ya existe".
