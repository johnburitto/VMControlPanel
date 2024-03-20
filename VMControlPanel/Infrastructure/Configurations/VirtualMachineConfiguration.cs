using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class VirtualMachineConfiguration : IEntityTypeConfiguration<VirtualMachine>
    {
        public void Configure(EntityTypeBuilder<VirtualMachine> builder)
        {
            builder.Property(_ => _.Id)
                   .IsRequired()
                   .UseIdentityColumn();

            builder.Property(_ => _.Name)
                   .IsRequired();

            builder.Property(_ => _.UserId)
                   .IsRequired();

            builder.Property(_ => _.UserName)
                   .IsRequired();

            builder.Property(_ => _.PasswordEncrypted)
                   .IsRequired();

            builder.Property(_ => _.Host)
                   .IsRequired();

            builder.Property(_ => _.Port)
                   .IsRequired();
            
            builder.Ignore(_ => _.Password);
        }
    }
}
