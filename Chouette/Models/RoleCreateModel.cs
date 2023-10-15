using System.ComponentModel.DataAnnotations;

namespace Chouette.Models
{
    public class RoleCreateModel
    {
        [Required(ErrorMessage ="Role adı gereklidir!")]
        public string Name { get; set; }
    }
}
