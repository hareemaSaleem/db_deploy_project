using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescribedMedicine> PrescribedMedicines { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicalRep> MedicalReps { get; set; }
        public DbSet<MedicineSample> MedicineSamples { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<VisitLog> VisitLogs { get; set; }
        public DbSet<PriorityQueue> PriorityQueues { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ClinicSettings> ClinicSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Patient>().HasKey(p => p.PatientId);
            modelBuilder.Entity<Doctor>().HasKey(d => d.DoctorId);
            modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmentId);
            modelBuilder.Entity<Billing>().HasKey(b => b.BillingId);
            modelBuilder.Entity<Prescription>().HasKey(p => p.PrescriptionId);
            modelBuilder.Entity<PrescribedMedicine>().HasKey(pm => pm.PrescribedMedicineId);
            modelBuilder.Entity<Medicine>().HasKey(m => m.MedicineId);
            modelBuilder.Entity<MedicalRep>().HasKey(mr => mr.MedicalRepId);
            modelBuilder.Entity<MedicineSample>().HasKey(ms => ms.SampleId);
            modelBuilder.Entity<Receptionist>().HasKey(r => r.ReceptionistId);
            modelBuilder.Entity<Admin>().HasKey(a => a.AdminId);
            modelBuilder.Entity<VisitLog>().HasKey(vl => vl.VisitLogId);
            modelBuilder.Entity<PriorityQueue>().HasKey(pq => pq.QueueId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<Billing>()
                .HasOne(b => b.Patient)
                .WithMany(p => p.Billings)
                .HasForeignKey(b => b.PatientId);

            modelBuilder.Entity<PrescribedMedicine>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.PrescribedMedicines)
                .HasForeignKey(pm => pm.PrescriptionId);

            modelBuilder.Entity<PrescribedMedicine>()
                .HasOne(pm => pm.Medicine)
                .WithMany(m => m.PrescribedMedicines)
                .HasForeignKey(pm => pm.MedicineId);
            modelBuilder.Entity<Patient>().HasIndex(p => p.CNIC).IsUnique();
            modelBuilder.Entity<Patient>().HasIndex(p => p.Email).IsUnique();
            modelBuilder.Entity<Doctor>().HasIndex(d => d.Email).IsUnique();
            modelBuilder.Entity<Receptionist>().HasIndex(r => r.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Medicine>().HasIndex(m => m.Name).IsUnique();
        }
    }
}