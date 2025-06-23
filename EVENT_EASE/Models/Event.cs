using System;
using System.Collections.Generic;

namespace EVENT_EASE.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Date { get; set; }

    public TimeOnly? Time { get; set; }

    public int? VenueId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Venue? Venue { get; set; }
    public int? EventTypeId { get; set; }
    public virtual EventType? EventType { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
