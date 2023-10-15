namespace Chouette.Models
{
    public class PointViewModel
    {

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public List<PointAndGame> Points { get; set; } // Change this to a List<int>
        public int GameNo { get; set; }
    }
    public class PointAndGame
    {
        public int Point { get; set; }
        public int GameNo { get; set; }
    }
}
