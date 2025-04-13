using System;

namespace Lapka.SharedModels.Base
{
    public enum AnimalType
    {
        None,
        Dog,
        Cat,
        Rabbit,
        Hamster,
        Parrot,
        Turtle,
        Snake,
        Other
    }
    public enum HealthStatus
    {
        None,
        Healthy,
        Sick,
        VerySick,
        Injured,
        Unknown
    }

    public class AnimalBase
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Person { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Breed { get; set; }
        public bool Sex { get; set; }
        public int Age { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public bool HasPassport { get; set; }
        public string HealthDescription { get; set; }
        public string Images { get; set; }
        public bool IsWarAnimal { get; set; }
        public bool IsSterilized { get; set; }
        public AnimalType Type { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public string Description { get; set; }
        public string DonationLink { get; set; }
        public DateTime AdditionDate { get; set; }
    }
}

