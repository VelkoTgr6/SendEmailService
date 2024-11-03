namespace SendEmailService.Data.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string TempPassword { get; set; }
    }
}
