using Lapka.SharedModels.Base;

namespace Lapka.SharedModels.DTO
{
    public class AnimalDTO : AnimalBase
    {
        public int Id { get; set; }
        public int ShelterId { get; set; }
    }
}

