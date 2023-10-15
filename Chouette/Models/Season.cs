using System.ComponentModel.DataAnnotations;

namespace Chouette.Models
{
    public class Season
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Lütfen sezon ismi giriniz!")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Lütfen tarih giriniz!")]
        [DataType(DataType.Date)]
        [Display(Name ="Geçerli bir tarih seçiniz!")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "Lütfen yer bilgisi giriniz!")]
        public string Place { get; set; }
        public int GameLimit { get; set; }

        public int TimeLimit { get; set; }

        public int OnePointValue { get; set; }

        public DateTime? CreateDate { get; set; }

        public ICollection<AppUser> Users { get; set; }=new List<AppUser>();
        public ICollection<Game> Games { get; set; }=new List<Game>();



    }
}
