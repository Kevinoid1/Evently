using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evently.Common.Infrastructure.Inbox;

public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
       builder.ToTable("inbox_messages");
       
       builder.HasKey(i => i.Id);

       builder.Property(i => i.Content).HasMaxLength(2000).HasColumnType("jsonb");
    }
}
