namespace EVENT_EASE.Models
{
    public partial class Venue
    {
        public int VenueId { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public int? Capacity { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable {  get; set; }
        
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}




