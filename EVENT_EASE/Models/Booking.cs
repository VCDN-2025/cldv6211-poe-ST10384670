using System;

namespace EVENT_EASE.Models;

public partial class Booking
{
    public int BookingId { get; set; }
    public DateTime? BookingDate { get; set; }

    public string? ClientName { get; set; }

    public string? ClientEmail { get; set; }

    public int? NumberOfGuests { get; set; }

    public int? EventId { get; set; }
    public virtual Event? Event { get; set; }

    public int? VenueId { get; set; }        // Nullable or not depending on your DB design
    public virtual Venue? Venue { get; set; }
}
