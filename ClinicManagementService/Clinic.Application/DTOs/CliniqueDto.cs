﻿
namespace Clinic.Application.DTOs
{
    public class CliniqueDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Adresse { get; set; }
        public string NumeroTelephone { get; set; }
        public string Email { get; set; }
        public DateTime DateCreation { get; set; }
    }
}
