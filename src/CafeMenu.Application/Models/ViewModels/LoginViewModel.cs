using System.ComponentModel.DataAnnotations;

namespace CafeMenu.Application.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre gereklidir")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

