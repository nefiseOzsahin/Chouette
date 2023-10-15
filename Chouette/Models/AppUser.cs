using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chouette.Models
{
    public class AppUser:IdentityUser<int>
    {

        public string ImagePath { get; set; }

        public string Gender { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public ICollection<Season> Seasons { get; set; }=new List<Season>();
        public ICollection<Game> Games { get; set; }=new List<Game>();
        public ICollection<Score> Scores { get; set; }=new List<Score>();


    }
}