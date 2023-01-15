namespace Pesutupa.Models.Exceptions
{
    public class NameAlreadyExists : Exception
    {
        public string Name { get; set; }

        public NameAlreadyExists(string name) : base($"Name {name} already exists.")
        {
            Name = name;
        }
    }

    public class EmailAlreadyExists : Exception
    {
        public string Email { get; set; }

        public EmailAlreadyExists(string email) : base($"Name {email} already exists.")
        {
            Email = email;
        }
    }
}
