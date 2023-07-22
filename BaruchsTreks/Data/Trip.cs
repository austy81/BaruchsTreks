namespace BaruchsTreks.Data
{
    public class Trip: DbEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // public string Country { get; set; } = string.Empty;
        // public string Difficulty { get; set; } = string.Empty;
        // public string SuitableDates { get; set; } = string.Empty;
        // public string EquipmentNeeded { get; set; } = string.Empty;
        public TimeSpan Length { get; set; }

        // Navigation property to represent the collection of ratings for this trip idea
    }

}
