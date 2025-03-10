using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder
                .ToTable("Orders");

            builder
                .HasKey(o =>  o.OrderId);

            builder
                .HasOne(o => o.Payment)
                .WithMany()
                .HasForeignKey(o => o.PaymentId);

            builder
                .HasOne(o => o.Coupon)
                .WithMany()
                .HasForeignKey(o => o.CouponId);

            builder
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);
        }
    }
}
