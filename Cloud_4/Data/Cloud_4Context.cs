using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cloud_4.Models;

namespace Cloud_4.Data
{
    public class Cloud_4Context : DbContext
    {
        public Cloud_4Context (DbContextOptions<Cloud_4Context> options)
            : base(options)
        {
        }

        public DbSet<Cloud_4.Models.Booking> Booking { get; set; } = default!;
        public DbSet<Cloud_4.Models.Event> Event { get; set; } = default!;
        public DbSet<Cloud_4.Models.Venue> Venue { get; set; } = default!;
        public DbSet<Cloud_4.Models.EventType> EventType { get; set; } = default!;
        public DbSet<Cloud_4.Models.EventBlob> EventBlobs { get; set; }
        public DbSet<Cloud_4.Models.VenueBlob> VenueBlobs { get; set; }

    }
}
