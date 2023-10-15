namespace Chouette.Models
{
    public class ScoreDetailViewModel
    {
        public Season? Season { get; set; }

        public int SeasonId { get; set; }
        public List<PointViewModel>? Users { get; set; }
    }
}
