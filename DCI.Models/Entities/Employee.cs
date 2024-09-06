using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.Entities
{
	public class Employee
	{
		public string Surname { get; set; } = string.Empty;
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string Middlename { get; set; } = string.Empty;
		public string ExtensionName { get; set; } = string.Empty;
		public DateTime DateofBirth { get; set; } = DateTime.Now;
		public string PlaceofBirth { get; set; } = string.Empty;
		public bool Sex { get; set; } = false;
		public int CivilStatus { get; set; } = 0;
		public string Height { get; set; } = string.Empty;
		public string Weight { get; set; } = string.Empty;
		public string BloodType { get; set; } = string.Empty;
		public string GSISNo { get; set; } = string.Empty;
		public string PagibigNo { get; set; } = string.Empty;
		public string PhilhealthNo { get; set; } = string.Empty;
		public string SSSNo { get; set; } = string.Empty;
		public string TIN { get; set; } = string.Empty;
		public string AgencyEmpNo { get; set; } = string.Empty;
		public int Citizenship { get; set; } = 0;
		public string CitizenshipCountry { get; set; } = string.Empty;
		public string Resi_HouseBlockLotNo { get; set; } = string.Empty;
		public string Resi_StreetNo { get; set; } = string.Empty;
		public string Resi_SubVillage { get; set; } = string.Empty;
		public string Resi_Barangay { get; set; } = string.Empty;
		public string Resi_City { get; set; } = string.Empty;
		public string Resi_Province { get; set; } = string.Empty;
		public string Resi_Zipcode { get; set; } = string.Empty;
		public string Perm_HouseBlockLotNo { get; set; } = string.Empty;
		public string Perm_StreetNo { get; set; } = string.Empty;
		public string Perm_SubVillage { get; set; } = string.Empty;
		public string Perm_Barangay { get; set; } = string.Empty;
		public string Perm_City { get; set; } = string.Empty;
		public string Perm_Province { get; set; } = string.Empty;
		public string Perm_Zipcode { get; set; } = string.Empty;
		public string Telephone { get; set; } = string.Empty;
		public string MobileNo { get; set; } = string.Empty;
		public string EmailNo { get; set; } = string.Empty;

		/*Family Background*/
		public string Spouse_Surname { get; set; } = string.Empty;
		public string Spouse_Firstname { get; set; } = string.Empty;
		public string Spouse_Middlename { get; set; } = string.Empty;
		public string Spouse_ExtensionName { get; set; } = string.Empty;
		
		public List<EmployeeChildren> ChildrenList { get; set; }

		public string Spouse_Occupation { get; set; } = string.Empty;
		public string Spouse_Employer { get; set; } = string.Empty;
		public string Spouse_EmployerAddress { get; set; } = string.Empty;
		public string Spouse_Telephone { get; set; } = string.Empty;

		public string Father_Surname { get; set; } = string.Empty;
		public string Father_Firstname { get; set; } = string.Empty;
		public string Father_Middlename { get; set; } = string.Empty;
		public string Father_ExtensionName { get; set; } = string.Empty;
		public string Mother_MaidenName { get; set; } = string.Empty;
		public string Mother_Surname { get; set; } = string.Empty;
		public string Mother_Firstname { get; set; } = string.Empty;
		public string Mother_Middlename { get; set; } = string.Empty;
		public string Mother_ExtensionName { get; set; } = string.Empty;

		/*Educationale Background*/
		public string Elementary_SchoolName { get; set; } = string.Empty;
		public string Elementary_DegreeCourse { get; set; } = string.Empty;
		public string Elementary_From { get; set; } = string.Empty;
		public string Elementary_To { get; set; } = string.Empty;
		public string Elementary_HighestLevelUnit { get; set; } = string.Empty;
		public string Elementary_YearGraduated { get; set; } = string.Empty;
		public string Elementary_AcademicAward { get; set; } = string.Empty;

		public string Secondary_SchoolName { get; set; } = string.Empty;
		public string Secondary_DegreeCourse { get; set; } = string.Empty;
		public string Secondary_From { get; set; } = string.Empty;
		public string Secondary_To { get; set; } = string.Empty;
		public string Secondary_HighestLevelUnit { get; set; } = string.Empty;
		public string Secondary_YearGraduated { get; set; } = string.Empty;
		public string Secondary_AcademicAward { get; set; } = string.Empty;

		public string Vocational_SchoolName { get; set; } = string.Empty;
		public string Vocational_DegreeCourse { get; set; } = string.Empty;
		public string Vocational_From { get; set; } = string.Empty;
		public string Vocational_To { get; set; } = string.Empty;
		public string Vocational_HighestLevelUnit { get; set; } = string.Empty;
		public string Vocational_YearGraduated { get; set; } = string.Empty;
		public string Vocational_AcademicAward { get; set; } = string.Empty;

		public string College_SchoolName { get; set; } = string.Empty;
		public string College_DegreeCourse { get; set; } = string.Empty;
		public string College_From { get; set; } = string.Empty;
		public string College_To { get; set; } = string.Empty;
		public string College_HighestLevelUnit { get; set; } = string.Empty;
		public string College_YearGraduated { get; set; } = string.Empty;
		public string College_AcademicAward { get; set; } = string.Empty;

		public string Graduate_SchoolName { get; set; } = string.Empty;
		public string Graduate_DegreeCourse { get; set; } = string.Empty;
		public string Graduate_From { get; set; } = string.Empty;
		public string Graduate_To { get; set; } = string.Empty;
		public string Graduate_HighestLevelUnit { get; set; } = string.Empty;
		public string Graduate_YearGraduated { get; set; } = string.Empty;
		public string Graduate_AcademicAward { get; set; } = string.Empty;
	}

	public class EmployeeChildren
	{
		public string ChildrenName { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; } = DateTime.Now;
	}
}
