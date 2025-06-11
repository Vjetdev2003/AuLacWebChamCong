using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AuLacChamCong.DataApi
{
    public class ApiContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Khai báo DTO không có khóa chính
            base.OnModelCreating(modelBuilder);

        }
        public ApiContext(DbContextOptions<ApiContext> options)
           : base(options)
        {

        }

    }
}
