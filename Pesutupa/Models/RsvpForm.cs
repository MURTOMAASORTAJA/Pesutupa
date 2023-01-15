using Microsoft.AspNetCore.Mvc;

namespace Pesutupa.Models
{
    /// <summary>
    /// Represents payload of the web form used for attempting to adding a new RSVP entry.
    /// </summary>
    public struct RsvpForm
    {
        /// <summary>
        /// Full name of the person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email address of the person.
        /// </summary>
        public string Email { get; set; }

        public RsvpForm(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public RsvpForm()
        {
            Name = "";
            Email = "";
        }
    }
}
