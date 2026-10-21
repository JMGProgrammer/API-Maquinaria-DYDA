using System.ComponentModel.DataAnnotations;

namespace API_Maquinaria_juan.DTOs
{
    // Body del PUT (actualización completa).
    // También hereda de EquipmentInputDTO, pero es una clase aparte a propósito:
    // las reglas del update van a divergir de las del create (ej: el Code podría
    // volverse no modificable). Tener los dos DTOs separados deja ese espacio.
    public class EquipmentUpdateDTO : EquipmentInputDTO
    {
    }
}
