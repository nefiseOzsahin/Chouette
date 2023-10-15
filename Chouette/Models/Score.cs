namespace Chouette.Models
{
    public class Score
    {
        public int Id { get; set; }

        public int Point { get; set; }

        public int AppUserId { get; set; }

        public AppUser? AppUser { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }
    }
}
