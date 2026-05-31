namespace ClinicManagementSystem.BL
{
    public class Validators
    {
            public static bool IsNameValid(string name, out string errorMessage)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    errorMessage = "Name is required";
                    return false;
                }

                if (name.Length < 2)
                {
                    errorMessage = "Name must be at least 2 characters";
                    return false;
                }

                if (name.Length > 100)
                {
                    errorMessage = "Name cannot exceed 100 characters";
                    return false;
                }

                errorMessage = "";
                return true;
            }
            public static bool IsCnicValid(string cnic, out string errorMessage)
            {
                if (string.IsNullOrWhiteSpace(cnic))
                {
                    errorMessage = "CNIC is required";
                    return false;
                }
                string cleanCnic = cnic.Replace("-", "");

                if (cleanCnic.Length != 13)
                {
                    errorMessage = "CNIC must be 13 digits";
                    return false;
                }
                foreach (char c in cleanCnic)
                {
                    if (!char.IsDigit(c))
                    {
                        errorMessage = "CNIC must contain only digits";
                        return false;
                    }
                }

                errorMessage = "";
                return true;
            }
            public static bool IsPhoneValid(string phone, out string errorMessage)
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    errorMessage = "Phone number is required";
                    return false;
                }
                if (phone.Length != 11)
                {
                    errorMessage = "Phone number must be 11 digits";
                    return false;
                }

                if (!phone.StartsWith("03"))
                {
                    errorMessage = "Phone number must start with 03";
                    return false;
                }
                foreach (char c in phone)
                {
                    if (!char.IsDigit(c))
                    {
                        errorMessage = "Phone number must contain only digits";
                        return false;
                    }
                }
                errorMessage = "";
                return true;
            }
            public static bool IsEmailValid(string email, out string errorMessage)
            {
                errorMessage = "";
                if (string.IsNullOrWhiteSpace(email))
                    return true; 
                if (!email.Contains("@") || !email.Contains("."))
                {
                    errorMessage = "Invalid email format";
                    return false;
                }
                if (email.IndexOf("@") == 0 || email.LastIndexOf(".") < email.IndexOf("@") + 2)
                {
                    errorMessage = "Invalid email format";
                    return false;
                }
                return true;
            }
            public static bool IsDateOfBirthValid(DateTime dateOfBirth, out string errorMessage)
            {
                if (dateOfBirth == default)
                {
                    errorMessage = "Date of Birth is required";
                    return false;
                }
                if (dateOfBirth > DateTime.Today)
                {
                    errorMessage = "Date of Birth cannot be in the future";
                    return false;
                }

                if (dateOfBirth < DateTime.Today.AddYears(-150))
                {
                    errorMessage = "Invalid Date of Birth";
                    return false;
                }

                errorMessage = "";
                return true;
            }
            public static bool IsAgeValid(int age, out string errorMessage)
            {
                if (age < 0)
                {
                    errorMessage = "Age cannot be negative";
                    return false;
                }
                if (age > 120)
                {
                    errorMessage = "Age cannot be more than 120 years";
                    return false;
                }
                errorMessage = "";
                return true;
            }
            public static bool IsGenderValid(string gender, out string errorMessage)
            {
                if (string.IsNullOrWhiteSpace(gender))
                {
                    errorMessage = "Gender is required";
                    return false;
                }
                if (gender != "Male" && gender != "Female" && gender != "Other")
                {
                    errorMessage = "Gender must be Male, Female, or Other";
                    return false;
                }
                errorMessage = "";
                return true;
            }
            public static bool IsMaritalStatusValid(string maritalStatus, out string errorMessage)
            {
                if (string.IsNullOrWhiteSpace(maritalStatus))
                {
                    errorMessage = "Marital Status is required";
                    return false;
                }
                string[] validStatuses = { "Single", "Married", "Divorced", "Widowed" };
                foreach (string status in validStatuses)
                {
                    if (maritalStatus == status)
                    {
                        errorMessage = "";
                        return true;
                    }
                }
                errorMessage = "Invalid Marital Status";
                return false;
            }
            public static bool IsBloodGroupValid(string bloodGroup, out string errorMessage)
            {
                errorMessage = "";

                if (string.IsNullOrWhiteSpace(bloodGroup))
                    return true;
                string[] validGroups = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                foreach (string group in validGroups)
                {
                    if (bloodGroup == group)
                        return true;
                }
                errorMessage = "Invalid Blood Group";
                return false;
            }
            public static bool IsAddressValid(string address, out string errorMessage)
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    errorMessage = "Address is required";
                    return false;
                }

                if (address.Length < 5)
                {
                    errorMessage = "Address must be at least 5 characters";
                    return false;
                }

                errorMessage = "";
                return true;
            }

            // ============== CITY VALIDATION ==============
            public static bool IsCityValid(string city, out string errorMessage)
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    errorMessage = "City is required";
                    return false;
                }

                if (city.Length < 2)
                {
                    errorMessage = "City must be at least 2 characters";
                    return false;
                }

                errorMessage = "";
                return true;
            }
            public static int CalculateAgeFromDateOfBirth(DateTime dateOfBirth)
            {
                int age = DateTime.Today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                    age--;
                return age;
            }
            public static bool DetermineRiskFlag(string chronicDiseases)
            {
                return !string.IsNullOrWhiteSpace(chronicDiseases);
            }
        public static bool IsDateNotInFuture(DateTime date, out string errorMessage)
        {
            if (date.Date < DateTime.Today)
            {
                errorMessage = "Date cannot be in the past";
                return false;
            }

            errorMessage = "";
            return true;
        }
    }
}