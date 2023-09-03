using BaruchsTreks.Enums;

namespace BaruchsTreks.Data
{
    public class Trip: DbEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int LengthHours { get; set; }

        public LocalGeometry Parking { get; set; } = new LocalGeometry();
        public LocalGeometry HighPoint { get; set; } = new LocalGeometry();

        public int MetersAscend { get; set; }
        public int? MetersDescend { get; set; }

        public UiaaGradeEnum? UiaaGrade { get; set; } = UiaaGradeEnum.IIIPlus;

        // Navigation property to represent the collection of ratings for this trip idea
    }

    public class LocalGeometry
    {
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
    }

}
