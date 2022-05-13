using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Core.Authentication.GoogleInteraction.ProfileReponse
{
    public class Metadata
    {
        public bool primary { get; set; }
        public Source source { get; set; }
        public bool sourcePrimary { get; set; }
    }
}
