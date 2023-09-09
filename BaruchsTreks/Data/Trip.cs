using BaruchsTreks.Enums;

namespace BaruchsTreks.Data
{
    public class Trip: DbEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal LengthHours { get; set; }

        public LocalGeometry Parking { get; set; } = new LocalGeometry();
        public LocalGeometry HighPoint { get; set; } = new LocalGeometry();

        public int MetersAscend { get; set; } = 0;
        public int MetersDescend { get; set; } = 0;

        public UiaaGradeEnum? UiaaGrade { get; set; } = UiaaGradeEnum.none;

        public AlpineGradeEnum? AlpineGrade { get; set;} = AlpineGradeEnum.none;

        public TripClassEnum? TripClass { get; set; } = TripClassEnum.none;

        public string? Participants { get; set; } = string.Empty;
    }

    public class LocalGeometry
    {
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
    }

}
