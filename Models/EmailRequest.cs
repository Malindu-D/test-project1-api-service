using System.ComponentModel.DataAnnotations;

namespace ApiServiceApp.Models;

public class EmailRequest
{
    [Required]
    [EmailAddress]
    public string ReceiverEmail { get; set; } = string.Empty;
}
