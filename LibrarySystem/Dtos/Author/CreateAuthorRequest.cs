using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.Author
{
    public class CreateAuthorRequest
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }
    }
}