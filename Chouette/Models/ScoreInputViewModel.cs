namespace Chouette.Models
{
    public class ScoreInputViewModel
    {

        public Season? Season { get; set; }

        public int SeasonId { get; set; }
        public List<UserViewModel>? Users { get; set; }
    }
}
