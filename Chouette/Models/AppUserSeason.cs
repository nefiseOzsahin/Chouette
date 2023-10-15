namespace Chouette.Models
{
    public class AppUserSeason
    {

        public int SeasonsId { get; set; }

        public Season Season { get; set; }

        public int UsersId { get; set; }

        public AppUser Users { get; set; }

    }
}
