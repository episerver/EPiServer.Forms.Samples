﻿using EPiServer.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Samples.Criteria.SelectionFactory;
using EPiServer.Globalization;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.VisitorGroups;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace EPiServer.Forms.Samples.Criteria
{
    public class FormModel : CriterionModelBase
    {
        [Ignore]
        public override EPiServer.Data.Identity Id { get; set; }

        [Required]
        [CriterionPropertyEditor(SelectionFactoryType = typeof(EnumSelectionFactory), LabelTranslationKey = "/episerver/forms/samples/criteria/formcriterion/submissionstatus", AdditionalPropsJson = "{ \"selectOnClick\": \"true\" }")]
        public SubmissionStatus SubmissionStatus { get; set; }

        [Required]
        [CriterionPropertyEditor(SelectionFactoryType = typeof(FormSelectionFactory), LabelTranslationKey = "/episerver/forms/samples/criteria/formcriterion/selectedform", AdditionalPropsJson = "{ \"selectOnClick\": \"true\" }")]
        public string SelectedForm { get; set; }

        public override ICriterionModel Copy()
        {
            // We can use the ShallowCopy method as this class
            // does not have any members that need to be deep copied
            return base.ShallowCopy();
        }
    }

    public enum SubmissionStatus
    {
        HasSubmitted,
        HasNotSubmitted
    }

    [VisitorGroupCriterion(
        Category = "EPiServer Forms",
        DisplayName = "Submitted Form",
        Description = "Criterion that checks if the visitor has submitted a specific Form in this session",
        LanguagePath = "/episerver/forms/samples/criteria/formcriterion")]
    public class FormCriterion : CriterionBase<FormModel>
    {
        /// <summary>
        /// Determines whether current user already submitted the form or not.
        /// </summary>
        public override bool IsMatch(IPrincipal principal, HttpContext httpContext)
        {
            var formGuid = Guid.Parse(Model.SelectedForm);
            var hasAlreadyPosted = HasAlreadyPosted(formGuid, httpContext);
            if (Model.SubmissionStatus == SubmissionStatus.HasSubmitted)
            {
                return hasAlreadyPosted;
            }
            else
            {
                return !hasAlreadyPosted;
            }
        }

        /// <summary>
        /// Check whether current user has already submitted the specified form.
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="httpContext"></param>
        private bool HasAlreadyPosted(Guid formId, HttpContext httpContext)
        {
            var psService = ServiceLocator.Current.GetInstance<ProgressiveSubmitInfoService>();
            var language = ContentLanguage.PreferredCulture.Name;
            var psInfo = psService.GetProgressiveSubmitInfo(formId, httpContext, language);
            if (psInfo != null && psInfo.IsFinalized)
            {
                return true;
            }

            return false;
        }
    }
}
