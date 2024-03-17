namespace DotnetAPI.Models
{
    public partial class User
    {
      public int UserId  {get; set;}
      public string FirstName {set; get;} ="";
      public string LastName {set; get;} ="";
      public string Email {set; get;} ="";
      public string Gender {set; get;} ="";
      public bool Active {set; get;}
    }
}