using System.ComponentModel.DataAnnotations;

namespace Chouette.Models
{
    public class Game
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Lütfen oyun ismi giriniz!")]
        public string Name { get; set; }
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();

        public int SeasonId { get; set; }

        public Season? Season { get; set; }

    }
}
