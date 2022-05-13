using System.Collections.Generic;

namespace Wolf.Utility.Core.Authentication.GoogleInteraction.ProfileReponse
{
    public class Root
    {
        public string resourceName { get; set; }
        public string etag { get; set; }
        public List<Name> names { get; set; }
    }


}
