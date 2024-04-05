namespace DotnetAPI.Models
{
    public partial class UserComplete
    {
      public int UserId  {get; set;}
      public string FirstName {set; get;} ="";
      public string LastName {set; get;} ="";
      public string Email {set; get;} ="";
      public string Gender {set; get;} ="";
      public bool Active {set; get;}
      public string JobTitle {set; get;} ="";
      public string Department {set; get;} ="";
      public double Salary {get; set;}
    }
}