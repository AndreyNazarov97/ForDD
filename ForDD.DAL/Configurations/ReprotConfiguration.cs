using ForDD.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForDD.DAL.Configurations
{
    public class ReprotConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired();

            builder.HasOne<User>()
                .WithMany(x => x.Reports)
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
