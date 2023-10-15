namespace Chouette.Models
{
    public class SeasonAddUser
    {

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();

        public Season Season { get; set; }
    }
}
