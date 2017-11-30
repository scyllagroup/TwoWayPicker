using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace TwoWayPicker.Core.Controllers
{
    [PluginController("TwoWayPicker")]
    public class TwoWayPickerApiController : UmbracoAuthorizedJsonController
    {
        public List<object> GetListOfContentTypeAliasesWithProperties()
        {
           // IEnumerable<string> memberTypeAliases = ApplicationContext.Services.MemberTypeService.GetAll().Select(x => x.Alias);
            //IEnumerable<string> mediaTypeAliases = ApplicationContext.Services.ContentTypeService.GetAllMediaTypes().Select(x => x.Alias);
            IEnumerable<string> contentTypeAliases = ApplicationContext.Services.ContentTypeService.GetAllContentTypeAliases();

            List<object> docTypeAliasesWithProperties = new List<object>();

            foreach(string docTypeAlias in contentTypeAliases.OrderBy(x => x))
            {
                docTypeAliasesWithProperties.Add(new { alias = docTypeAlias, propertyAliases = GetPropertyAliasesFromContentType(docTypeAlias) });
            }

            return docTypeAliasesWithProperties;//.Except(mediaTypeAliases).Except(memberTypeAliases);
        }

        public IEnumerable<string> GetPropertyAliasesFromContentType(string alias)
        {
            IContentType contentType = ApplicationContext.Services.ContentTypeService.GetContentType(alias);

            if (contentType == null)
            {
                try
                {
                    IMemberType memberType = ApplicationContext.Services.MemberTypeService.GetAll().Where(x => x.Alias.Equals(alias)).FirstOrDefault();
                    if (memberType == null) return Enumerable.Empty<string>();
                    return memberType.PropertyTypes.Where(x => x.PropertyEditorAlias.Equals("TwoWayPicker")).Select(x => x.Alias);
                }
                catch
                {
                    return Enumerable.Empty<string>();
                }
                
            }
            
            return contentType.PropertyTypes.Where(x => x.PropertyEditorAlias.Equals("TwoWayPicker")).Select(x => x.Alias).OrderBy(y => y);
        }
    }
}