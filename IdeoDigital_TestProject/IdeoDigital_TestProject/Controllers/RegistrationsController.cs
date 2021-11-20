using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using IdeoDigital_TestProject.Models;
using LightInject;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
namespace IdeoDigital_TestProject.Controllers 
{
    public class RegistrationsController : UmbracoApiController
    {
        [HttpPost]
        public void Register(RegisterPostModel model)
        {
            if(!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            var memberService = Services.MemberService;
            if(memberService.GetByEmail(model.Email) != null)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            var member = memberService.CreateMemberWithIdentity(model.Email, model.Email, model.FirstName, "myMember");
            memberService.SavePassword(member, model.Password);
            member.SetValue("firstName", model.FirstName);
            member.SetValue("lastName", model.LastName);
            member.SetValue("phone", model.Phone);
            memberService.Save(member);         
        }
    }
}