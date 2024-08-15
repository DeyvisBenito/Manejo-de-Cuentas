using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength:50, MinimumLength =3, ErrorMessage =("El campo {0} tiene que estar entre {2} y {1} caracteres."))]
        [Display(Name ="Tipo de cuenta")]
        [PrimeraLetraMayuscula]
        [Remote(action: "existe", controller:"TiposCuentas")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }
    }
}
