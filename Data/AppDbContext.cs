using Microsoft.EntityFrameworkCore;
using StudentRosterApi.Models;

namespace StudentRosterApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasOne(student => student.Department)
            .WithMany(department => department.Students)
            .HasForeignKey(student => student.DepartmentId)
            .IsRequired(false);

        modelBuilder.Entity<Student>()
            .HasMany(student => student.Courses)
            .WithMany(course => course.Students);
    }
}
