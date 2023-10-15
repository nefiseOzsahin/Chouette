using Microsoft.AspNetCore.Identity;

namespace Chouette.Models
{
    public class AppRole:IdentityRole<int>
    {

        public DateTime CreatedTime { get; set; }
    }
}
