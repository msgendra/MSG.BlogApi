using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSG.BlogApi.Domain;

namespace MSG.BlogApi.Infrastructure.Persistence.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(c => c.Author).HasMaxLength(100);
        builder.Property(c => c.Content).HasMaxLength(2000).IsRequired();
    }
}
