namespace DotnetAPI.Dtos
{


    public partial class UserForRegistrationDTO
    {
        public string Email {get; set;} = "";

        public string Password {get; set;} = "";

        public string PasswordConfirm {get; set;} = "";

        public string FirstName {set; get;} ="";

        public string LastName {set; get;} ="";
        
        public string Gender {set; get;} ="";

        public string JobTitle {set; get;} ="";
        public string Department {set; get;} ="";
        public double Salary {get; set;}
    }

}