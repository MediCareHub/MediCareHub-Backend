﻿// <auto-generated />
using System;
using MediCareHub.DAL.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MediCareHub.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MediCareHub.DAL.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentId"));

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AppointmentId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentId"));

                    b.Property<string>("DepartmentName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.Doctor", b =>
                {
                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ExperienceYears")
                        .HasColumnType("int");

                    b.Property<string>("Qualification")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("DoctorId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.DoctorAvailability", b =>
                {
                    b.Property<int>("AvailabilityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AvailabilityId"));

                    b.Property<string>("DayOfWeek")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time");

                    b.HasKey("AvailabilityId");

                    b.HasIndex("DoctorId");

                    b.ToTable("DoctorAvailability");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.MedicalRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecordId"));

                    b.Property<int>("AppointmentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Diagnosis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Medication")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RecordId");

                    b.HasIndex("AppointmentId")
                        .IsUnique();

                    b.ToTable("MedicalRecords");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.Patient", b =>
                {
                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("Allergies")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrentMedications")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmergencyContact")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("MedicalHistory")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PatientId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.Appointment", b =>
                {
                    b.HasOne("MediCareHub.DAL.Models.Doctor", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MediCareHub.DAL.Models.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.Doctor", b =>
                {
                    b.HasOne("MediCareHub.DAL.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MediCareHub.DAL.Models.User", "User")
                        .WithOne()
                        .HasForeignKey("MediCareHub.DAL.Models.Doctor", "DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.DoctorAvailability", b =>
                {
                    b.HasOne("MediCareHub.DAL.Models.Doctor", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.MedicalRecord", b =>
                {
                    b.HasOne("MediCareHub.DAL.Models.Appointment", "Appointment")
                        .WithOne("MedicalRecord")
                        .HasForeignKey("MediCareHub.DAL.Models.MedicalRecord", "AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Appointment");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.Patient", b =>
                {
                    b.HasOne("MediCareHub.DAL.Models.User", "User")
                        .WithOne()
                        .HasForeignKey("MediCareHub.DAL.Models.Patient", "PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MediCareHub.DAL.Models.Appointment", b =>
                {
                    b.Navigation("MedicalRecord")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
