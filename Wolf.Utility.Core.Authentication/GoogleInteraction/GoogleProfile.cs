using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Core.Authentication.GoogleInteraction
{
    public class GoogleProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Email { get; set; }
        public string Verified_Email { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{nameof(Id)} : {Id} | ");
            builder.Append($"{nameof(Name)} : {Name} | ");
            builder.Append($"{nameof(Picture)} : {Picture} | ");
            builder.Append($"{nameof(Email)} : {Email} | ");
            builder.Append($"{nameof(Verified_Email)} : {Verified_Email}");

            return builder.ToString();
        }
    }
}