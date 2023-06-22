using EPiServer.Forms.Samples.Business;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc;
namespace EPiServer.Forms.Samples.Controllers
{
    [Route("ExternalValidate")]
    public class ExternalValidateController: Controller
    {
        Injected<IAddressValidateService> _addressService;
        /// <summary>
        /// Validate form address element.
        /// </summary>
        /// <param name="address">The address to validate.</param>
        [HttpPost]
        public JsonResult ValidateAddress(string address, string street, string city, string state, string postalCode, string country, bool ignoreDetail = false)
        {
            var validateResult = _addressService.Service.Validate(address, street, city, state, postalCode, country, ignoreDetail);

            return Json(validateResult);
        }
    }
}
