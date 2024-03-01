using System;
using System.ComponentModel.DataAnnotations;

namespace ProductDatabaseAPI.Dtos;

public class UserRegistrationRequestDto
{
	[Required]
	public string Name { get; set; } = String.Empty;
	[Required]
        public string Email { get; set; } = String.Empty;
	[Required]
    public string Password { get; set; } = String.Empty;
}

