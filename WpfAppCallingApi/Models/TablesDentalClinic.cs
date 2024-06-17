using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;


namespace WpfAppCallingApi.Models
{

    [Table("Consumption")]
    public class Consumption
    {
        [Column("ID_Consumption")]
        public int Id { get; set; }

        [Column("ID_Drug")]
        public int idDrug { get; set; }

        [Column("ID_Department")]
        public int idDepartment { get; set; }

        [Column("Use_date")]
        public DateTime useDate { get; set; }

        [Column("Quantity_used")]
        public string quantityUsed { get; set; }

        [Column("ID_User")]
        public int idUser { get; set; }
    }

    [Table("Departments")]
    public class Departments
    {
        [Column("ID_Department")]
        public int Id { get; set; }

        [Column("Name")]
        public string name { get; set; }

        [Column("Responsible_employee")]
        public string responsibleEmployee { get; set; }
    }

    [Table("Drugs")]
    public class Drugs
    {
        [Column("ID_Drug")]
        public int Id { get; set; }

        [Column("Name")]
        public string name { get; set; }

        [Column("Classification")]
        public string classification { get; set; }

        [Column("Dosage")]
        public string dosage { get; set; }

        [Column("Manufacturer")]
        public string manufacturer { get; set; }

        [Column("Country_of_origin")]
        public string countryOfOrigin { get; set; }
    }

    [Table("Inventory")]
    public class Inventory
    {
        [Column("ID_Inventory")]
        public int Id { get; set; }

        [Column("ID_Drug")]
        public int idDrag { get; set; }

        [Column("ID_Department")]
        public int idDepartment { get; set; }

        [Column("Inventory_check_date")]
        public DateTime inventoryCheckDate { get; set; }

        [Column("Remaining_quantity_of_drugs")]
        public string remainingQuantityOfDrugs { get; set; }

        [Column("Storage_location")]
        public string storageLocation { get; set; }
    }

    [Table("Suppliers")]
    public class Suppliers
    {
        [Column("ID_Supplier")]
        public int Id { get; set; }

        [Column("Company_name")]
        public string companyName { get; set; }

        [Column("Contact_info")]
        public string contactInfo { get; set; }

        [Column("Supply_country")]
        public string supplyCountry { get; set; }
    }

    [Table("Supply")]
    public class Supply
    {
        [Column("ID_Supply")]
        public int Id { get; set; }

        [Column("ID_Drug")]
        public int idDrug { get; set; }

        [Column("ID_Supplier")]
        public int idSupplier { get; set; }

        [Column("Supply_date")]
        public DateTime supplyDate { get; set; }

        [Column("Quantity")]
        public string quantity { get; set; }

        [Column("Cost")]
        public string cost { get; set; }
    }


    [Table("Users")]
    public class Users
    {
        [Column("ID_User")]
        public int Id { get; set; }

        [Column("Name")]
        public string fullName { get; set; }

        [Column("Position")]
        public string position { get; set; }

        [Column("Access_rights")]
        public string accessRights { get; set; }
    }
}
