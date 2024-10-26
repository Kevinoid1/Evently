using Evently.Modules.Attendance.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evently.Modules.Attendance.Infrastructure.Events;

public class EventStatisticsConfiguration : IEntityTypeConfiguration<EventStatistics>
{
    public void Configure(EntityTypeBuilder<EventStatistics> builder)
    {
        builder.HasKey(es => es.EventId);
        
        builder.Property(es => es.EventId).ValueGeneratedNever();
    }
}
