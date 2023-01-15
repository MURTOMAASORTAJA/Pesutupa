using Microsoft.Extensions.Primitives;
using Pesutupa.Models;

namespace Pesutupa
{
    public static class Rsvp
    {
        public static readonly string AllowedRsvpNameCharacters = "aeiouyåäö'-ñìíóòãáà ";
        public static readonly string AllowedRsvpEmailCharacters = "aeiouyåäö-.1234567890@";

        public static async Task<IResult> ProcessPostRsvp(HttpContext ctx)
        {
            RsvpForm form;

            #region Validation stuff
            
            if (!ctx.Request.HasFormContentType || ctx.Request.ContentLength > 365 || !TryDeserializeRsvpForm(ctx.Request.Form, out form))
            {
                return Results.BadRequest();
            }

            if (!RsvpFormNameIsValid(form))
            {
                return Results.BadRequest("Invalid name.");
            }

            if (!RsvpFormEmailIsValid(form))
            {
                return Results.BadRequest("Invalid email.");
            }

            #endregion

            return Results.Ok();
        }

        public static bool TryDeserializeRsvpForm(IFormCollection formFromRequest, out RsvpForm form)
        {
            StringValues nameValues;
            form = default;

            if (!formFromRequest.TryGetValue("name", out nameValues)) {
                return false;
            }

            StringValues emailValues;

            if (!formFromRequest.TryGetValue("email", out emailValues))
            {
                return false;
            }

            if (nameValues.Count() != 1 || emailValues.Count() != 1)
            {
                return false;
            }

            var name = nameValues.First();

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            
            var email = emailValues.First();

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email can't be empty.");
            }

            form = new RsvpForm(name, email);
            return true;
        }

        public static bool RsvpFormNameIsValid(RsvpForm form)
        {
            return !(string.IsNullOrEmpty(form.Name) || form.Name.Any(character => !AllowedRsvpNameCharacters.Contains(character)));
        }

        public static bool RsvpFormEmailIsValid(RsvpForm form) 
        {
            return !(string.IsNullOrEmpty(form.Name) || form.Name.Any(character => !AllowedRsvpEmailCharacters.Contains(character)));
        }

        /// <summary>
        /// Not used for anything at this moment.
        /// </summary>
        public static T? Deserialize<T>(IFormCollection formFromRequest) where T: new()
        {
            var type = typeof(T);
            var result = new T();

            foreach (var propInfo in type.GetProperties())
            {
                var propName = propInfo.Name.ToLowerInvariant();
                if (formFromRequest.ContainsKey(propName))
                {
                    var valueInForm = formFromRequest[propName].First();
                    if (!string.IsNullOrEmpty(valueInForm))
                    {
                        if (propInfo.PropertyType != typeof(string))
                        {
                            var convertedValue = Convert.ChangeType(valueInForm, propInfo.PropertyType);
                            propInfo.SetValue(result, convertedValue);
                        } else
                        {
                            propInfo.SetValue(result, valueInForm);
                        }
                    }
                }
            }

            return result;
        }
    }
}
