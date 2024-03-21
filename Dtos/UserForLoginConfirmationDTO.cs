namespace DotnetAPI.Dtos
{
    public partial class UserForLoginConfirmationDTO
    {
        public byte[] PasswordHash {get;set;}
        public byte[] PasswordSalt {get; set;}

        public UserForLoginConfirmationDTO()
        {
            if(PasswordHash == null)
            {
                PasswordHash = Array.Empty<byte>();
            }
            if(PasswordSalt == null)
            {
                PasswordSalt = Array.Empty<byte>();
            }
        }

    }
}