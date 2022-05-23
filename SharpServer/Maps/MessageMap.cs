using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpServer.Models;

namespace SharpServer
{
    public class MessageMap
    { 
        public MessageMap(EntityTypeBuilder<Message> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);
            entityTypeBuilder.ToTable("message");

            entityTypeBuilder.Property(x => x.Id).HasColumnName("id");
            entityTypeBuilder.Property(x => x.Sender).HasColumnName("sender");
            entityTypeBuilder.Property(x => x.Content).HasColumnName("content");
        }
    }
}