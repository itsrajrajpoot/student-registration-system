namespace StudentRegistrationSystem.ViewModels
{
    public class UserWithRolesViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public List<string> Roles { get; set; }
    }
}