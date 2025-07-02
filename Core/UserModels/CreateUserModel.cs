using System.ComponentModel.DataAnnotations;
using Core.Entity.Helper;

namespace Core.UserModels
{
    public class CreateUserModel
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "Le prénom est réquis!")]
        public string? Prenom { get; set; }
        [Required(ErrorMessage = "Le nom de famille est réquis!")]
        public string? NomDeFamille { get; set; }
        [Required(ErrorMessage = "Le nom d'utilisateur / Identifiant est réquis!")]
        public string Identifiant { get; set; }
        [Required(ErrorMessage = "Le rôle est réquis!")]
        public string? Role { get; set; }

        [CreateUserPasswordValidator(5)]
        public string? Password { get; set; }
        public string? UserRoleId { get; set; }
        [Required(ErrorMessage = "Le site est réquis!")]
        public bool UpdatePassword { get; set; }

        public string DisplayName => $"{NomDeFamille} {Prenom}";
        public string Icon => $"{NomDeFamille?.FirstOrDefault()}{Prenom?.FirstOrDefault()}".PadLeft(1, 'X').ToUpper();
    }
}