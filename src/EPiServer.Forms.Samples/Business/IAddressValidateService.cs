using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Forms.Samples.Business
{
    public interface IAddressValidateService
    {
        bool Validate(string address, string street, string city, string state, string postalCode, string country, bool ignoreDetail);
    }
}
