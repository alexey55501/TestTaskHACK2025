using System.ComponentModel.DataAnnotations;

namespace Lapka.SharedModels.Base
{
    public enum ShelterType
    {
        Vetclinic,
        Shelter,
        Breeder,
        Other,
    }
    public class ShelterBase
    {
        //[Required(ErrorMessage = "Адреса є обов'язковою")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "Тип притулку є обов'язковим")]
        public ShelterType Type { get; set; }

        //[Required(ErrorMessage = "Інформація про притулок є обов'язковою")]
        public string About { get; set; }

        //[Required(ErrorMessage = "Номер телефону є обов'язковим")]
        [Phone(ErrorMessage = "Невірний формат номера телефону")]
        public string? Phone { get; set; }

        //[Required(ErrorMessage = "Посилання на донати є обов'язковим")]
        public string DonationLink { get; set; }
    }
}

