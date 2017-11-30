using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwoWayPicker.Core.Helpers;
using TwoWayPicker.Core.Helpers.Implementations;
using TwoWayPicker.Core.Helpers.Interfaces;
using TwoWayPicker.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace TwoWayPicker.Core.App_Start
{
    public class AppStartEvents : ApplicationEventHandler


    {
        public AppStartEvents():base()
        {


            ContentService.Saved += ContentService_Saved;
            MemberService.Saved += MemberService_Saved;
            MemberService.Saving += MemberService_Saving;
        }

        private void MemberService_Saving(IMemberService sender, SaveEventArgs<IMember> e)
        {
            var twpEntities = e.SavedEntities.Where(x => x.PropertyTypes.Any(y => y.PropertyEditorAlias.Equals("TwoWayPicker")));
            
            //Dirty hack but for the two way picker to function, we need to read the previous value of the picker. With members that is difficult because the content cache is immediately updated after
            //this "Saving" event. The value is erased by the time we are in the saved event. For content, the cache is updated a little later so we can do Umbraco.TypedContent for content in the "Saved" event
            //we get the old value before the xml cache is updated. 
            foreach(IMember member in twpEntities)
            {
                //Grab the properties from the member that are a two way picker so we can get their initial value to save. 
                var twpProperties = member.PropertyTypes.Where(x => x.PropertyEditorAlias.Equals("TwoWayPicker"));
                MembershipHelper mh = new MembershipHelper(UmbracoContext.Current);
                IPublishedContent memberOldData = mh.GetById(member.Id);

                foreach(var property in twpProperties)
                {
                    string value;

                    //Core value converter case, we need a comma separated string of ids, it wants to return IEnumberabl<IPublishedContent>
                    IEnumerable<IPublishedContent> pickedContent = memberOldData.GetPropertyValue<IEnumerable<IPublishedContent>>(property.Alias);

                    //Interesting, if we have the Our Core Property Value Converters package installed, this will return null, so check to see if 
                    //we can get it back as an IEnumerable<IPublishedContent>
                    if (pickedContent == null || !pickedContent.Any())
                    {
                        value = memberOldData.GetPropertyValue<string>(property.Alias);
                    }
                    else
                    {
                        value = string.Join(",", pickedContent.Select(x => x.Id.ToString()));
                    }
                    MemberPreviousValueRepo.Instance.SaveValue(member.Id.ToString(), property.Alias, value);
                }
            }

        }

        private void MemberService_Saved(IMemberService sender, SaveEventArgs<IMember> e)
        {
            //Entities with a two way picker attached.
            var twpEntities = e.SavedEntities.Where(x => x.PropertyTypes.Any(y => y.PropertyEditorAlias.Equals("TwoWayPicker")));
            var dts = ApplicationContext.Current.Services.DataTypeService;
            foreach (var content in twpEntities)
            {
                //We need to see what properties contain the TwoWayPicker and then inspect their prevalues. 
                //The prevalues will contain the doc type aliases, as well as their two way picker property alias
                var twpProperties = content.PropertyTypes.Where(x => x.PropertyEditorAlias.Equals("TwoWayPicker"));

                if (twpProperties == null || !twpProperties.Any()) continue;

                foreach (var property in twpProperties)
                {
                    PreValueCollection pv = dts.GetPreValuesCollectionByDataTypeId(property.DataTypeDefinitionId);
                    var pvMap = pv.PreValuesAsDictionary["docTypePropertyMap"];
                    StartNodePrevalueModel typeModel = JsonConvert.DeserializeObject<StartNodePrevalueModel>(pv.PreValuesAsDictionary["startNode"].Value);
                    IEnumerable<TwoWayPickerPrevalueModel> prevalues = JsonConvert.DeserializeObject<IEnumerable<TwoWayPickerPrevalueModel>>(pvMap.Value);

                    IUmbracoServiceWrapper serviceWrapper;
                    if (typeModel.type == "member")
                    {
                        serviceWrapper = new UmbracoMembersServiceWrapper(sender);
                    }
                    else
                    {
                        serviceWrapper = serviceWrapper = new UmbracoContentServiceWrapper(ApplicationContext.Current.Services.ContentService);
                    }
                    foreach (TwoWayPickerPrevalueModel prevalue in prevalues)
                    {
                            TwoWayPickerUtility.Instance.UpdateOppositePicker(property.Alias, prevalue.SelectedProperty, content, new OldPickedValuesFromMemberProvider(content), serviceWrapper);
                    }
                }
            }
        }

        void ContentService_Saved(IContentService sender, SaveEventArgs<IContent> e)
        {
            //Entities with a two way picker attached.
            var twpEntities = e.SavedEntities.Where(x => x.PropertyTypes.Any(y => y.PropertyEditorAlias.Equals("TwoWayPicker")));
            var dts = ApplicationContext.Current.Services.DataTypeService;
            foreach (var content in twpEntities)
            {
                //We need to see what properties contain the TwoWayPicker and then inspect their prevalues. 
                //The prevalues will contain the doc type aliases, as well as their two way picker property alias
                var twpProperties = content.PropertyTypes.Where(x => x.PropertyEditorAlias.Equals("TwoWayPicker"));

                if (twpProperties == null || !twpProperties.Any()) continue;

                foreach (var property in twpProperties)
                {
                    PreValueCollection pv = dts.GetPreValuesCollectionByDataTypeId(property.DataTypeDefinitionId);
                    var pvMap = pv.PreValuesAsDictionary["docTypePropertyMap"];
                    StartNodePrevalueModel typeModel = JsonConvert.DeserializeObject<StartNodePrevalueModel>(pv.PreValuesAsDictionary["startNode"].Value);
                    IEnumerable<TwoWayPickerPrevalueModel> prevalues = JsonConvert.DeserializeObject<IEnumerable<TwoWayPickerPrevalueModel>>(pvMap.Value);

                    IUmbracoServiceWrapper serviceWrapper;
                    if(typeModel.type == "member")
                    {
                        serviceWrapper = new UmbracoMembersServiceWrapper(ApplicationContext.Current.Services.MemberService);
                    }
                    else
                    {
                        serviceWrapper = serviceWrapper = new UmbracoContentServiceWrapper(sender);
                    }
                    foreach (TwoWayPickerPrevalueModel prevalue in prevalues)
                    {
                        TwoWayPickerUtility.Instance.UpdateOppositePicker(property.Alias, prevalue.SelectedProperty, content, new OldPickedValuesFromContentProviderProvider(new UmbracoHelper(UmbracoContext.Current)), serviceWrapper);
                    }
                }
            }
        }
    }

    public class StartNodePrevalueModel
    {
        public string type { get; set; }
        public int id { get; set; }

        public string query { get; set; }
    }
}