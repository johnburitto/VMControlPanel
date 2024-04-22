using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UserInfrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(_ =>  _.Id);

            builder.Property(_ => _.Id)
                   .IsRequired();

            builder.Property(_ => _.TelegramId)
                   .IsRequired();
            
            builder.Property(_ => _.UserName)
                   .IsRequired();
            
            builder.Property(_ => _.PasswordHash)
                   .IsRequired();
            
            builder.Property(_ => _.Email)
                   .IsRequired();
            
            builder.Property(_ => _.NormalizedUserName)
                   .IsRequired();

            builder.Property(_ => _.NormalizedEmail)
                   .IsRequired();
            
            builder.Property(_ => _.Culture)
                   .HasConversion(new EnumToStringConverter<Cultures>())
                   .IsRequired();
        }
    }
}
