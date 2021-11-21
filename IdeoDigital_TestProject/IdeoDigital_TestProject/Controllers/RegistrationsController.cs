using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using IdeoDigital_TestProject.Extensions;
using IdeoDigital_TestProject.Models;
using LightInject;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
namespace IdeoDigital_TestProject.Controllers 
{
    public class RegistrationsController : UmbracoApiController
    {

        [HttpPost]
        public object Register(RegisterPostModel model)
        {
            if(!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var memberService = Services.MemberService;
            IMedia media = null;

            if (memberService.GetByEmail(model.Email) != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            if (!string.IsNullOrEmpty(model.IconEncoded))
            {
                try
                {
                    var bytes = Convert.FromBase64String(model.IconEncoded);
                    var generatedMediaName = GenerateMediaName(model);
                    media = CreateMediaFromBytes(bytes, generatedMediaName);
                }
                catch (FormatException)
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
            }

            var fullName = $"{model.FirstName} {model.LastName}";
            var member = memberService.CreateMember(model.Email, model.Email, fullName, "myMember");

            member.SetValue("firstName", model.FirstName);
            member.SetValue("lastName", model.LastName);
            member.SetValue("phone", model.Phone);

            if (media != null)
            {
                member.SetValue("icon", media.GetMediaReference());
            }

            memberService.Save(member); 
            memberService.SavePassword(member, model.Password);

            return Ok();   
        }


        private IMedia CreateMediaFromBytes(byte[] bytes, string fileName)
        {   
            var mediaService = Services.MediaService;
            using (var stream = new MemoryStream(bytes))
            {
                var media = mediaService.CreateMedia(fileName, Constants.System.Root, "Image");

                media.SetValue(Services.ContentTypeBaseServices, "umbracoFile", fileName, stream);
                mediaService.Save(media);

                return media;
            }
        }

        private string GenerateMediaName(RegisterPostModel model)
        {
            return $"{model.Email}_icon.jpg";
        }
    }
}