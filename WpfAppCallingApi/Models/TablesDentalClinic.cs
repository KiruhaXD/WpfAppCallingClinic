using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;


namespace WpfAppCallingApi.Models
{
    [Table("Patients")]
    public class Patients
    {
        [Column("ID_Patient")]
        public int Id { get; set; }

        [Column("Full_name")]
        public string fullName { get; set; }

        [Column("Data_birth")]
        public DateTime dateBirth { get; set; }

        [Column("Phone_number")]
        public string phoneNumber { get; set; }

        [Column("Email")]
        public string email { get; set; }

        [Column("Address")]
        public string address { get; set; }
    }

    [Table("Specialization_doctor")]
    public class Specialization_doctor
    {
        [Column("ID_Specialty")]
        public int Id { get; set; }

        [Column("Name_specialty")]
        public string Name_specialty { get; set; }
    }

    [Table("Doctors")]
    public class Doctors
    {
        [Column("ID_Doctor")]
        public int Id { get; set; }

        [Column("Full_name")]
        public string fullName { get; set; }

        [Column("ID_Specialty")]
        public int idSpecialty { get; set; }

        [Column("Contact_info")]
        public string contactInfo { get; set; }

        [Column("Medical_experience")]
        public string medicalExperience { get; set; }

        [Column("Work_schedule")]
        public string workSchedule { get; set; }
    }

    [Table("Records_on_receiving")]
    public class Records_on_receiving
    {
        [Column("ID_Record")]
        public int Id { get; set; }

        [Column("ID_Destination")]
        public int idDestination { get; set; }

        [Column("ID_Services")]
        public int idServices { get; set; }

        [Column("Data_and_time_record")]
        public DateTime dataAndTimeRecord { get; set; }

        [Column("ID_Doctor")]
        public int idDoctor { get; set; }

        [Column("ID_Patient")]
        public int idPatient { get; set; }

        [Column("ID_Receiving_type")]
        public int IdReceivingType { get; set; }
    }

    [Table("Destinations")]
    public class Destination
    {
        [Column("ID_Destination")]
        public int Id { get; set; }

        [Column("Data_destination")]
        public DateTime dataDestination { get; set; }

        [Column("Description_destination")]
        public string descriptionDestination { get; set; }
    }

    [Table("Services")]
    public class Services
    {
        [Column("ID_Services")]
        public int Id { get; set; }

        [Column("Name_service")]
        public string nameService { get; set; }

        [Column("Description_service")]
        public string descriptionService { get; set; }

        [Column("Price")]
        public decimal price { get; set; }
    }


    [Table("List_of_diseases")]
    public class List_of_diseases
    {
        [Column("ID_List_of_diseases")]
        public int Id { get; set; }

        [Column("Diseases")]
        public string diseases { get; set; }

        [Column("Treatment")]
        public string treatment { get; set; }
    }

    [Table("Patient_card")]
    public class Patient_card
    {
        [Column("ID_Patient_card")]
        public int Id { get; set; }

        [Column("ID_Patient")]
        public int idPatient { get; set; }

        [Column("ID_List_of_diseases")]
        public int idListOfDiseases { get; set; }

        [Column("Data_record")]
        public DateTime dataRecord { get; set; }

        [Column("Description_patient")]
        public string descriptionPatient { get; set; }

        [Column("Procedures_and_treatments")]
        public string proceduresAndTreatments { get; set; }
    }

    [Table("Receiving_type")]
    public class Receiving_type
    {
        [Column("ID_Receiving_type")]
        public int Id { get; set; }

        [Column("Name_receiving")]
        public string nameReceiving { get; set; }
    }
}
