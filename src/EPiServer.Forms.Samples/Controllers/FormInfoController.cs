using EPiServer.Core;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Data;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Forms.Helpers;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Forms.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Forms.Samples.Controllers
{
    [Authorize(Roles = "CmsAdmins, VisitorGroupAdmins")]
    public class FormInfoController: Controller
    {
        private Injected<IFormRepository> _formRepository;
        Injected<IContentRepository> _contentRepository;

        /// <summary>
        /// Get friendly names of a specified form.
        /// </summary>
        /// <param name="formGuid">The form unique identifier.</param>
        public JsonResult GetElementFriendlyNames(Guid formGuid)
        {
            IContentData content;
            if (!_contentRepository.Service.TryGet<IContentData>(formGuid, out content))
            {
                return Json(null);
            }

            var formContainerBlock = content as FormContainerBlock;
            if (formContainerBlock == null)
            {
                return Json(null);
            }

            var formIden = new FormIdentity(formGuid, (content as ILocalizable).Language.Name);
            var friendlyNames = _formRepository.Service.GetDataFriendlyNameInfos(formIden)
                                    .Where(fn => !fn.ElementId.StartsWith(EPiServer.Forms.Constants.SYSTEMCOLUMN_PREFIX))
                                    .OrderBy(fn => fn.FriendlyName);

            return Json(friendlyNames);
        }
    }
}
