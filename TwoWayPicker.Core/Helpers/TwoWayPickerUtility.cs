using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using TwoWayPicker.Core.Extensions;
using log4net;
using System.Reflection;
using Umbraco.Core.Services;
using TwoWayPicker.Core.Helpers.Interfaces;

namespace TwoWayPicker.Core.Helpers
{
    public class TwoWayPickerUtility
    {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static TwoWayPickerUtility _instance;

        private TwoWayPickerUtility()
        {

        }

        public static TwoWayPickerUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TwoWayPickerUtility();
                }
                return _instance;
            }
        }

        public bool UpdateOppositePicker(string thisPickerPropertyAlias, string oppositePickerPropertyAlias, IEnumerable<IContentBase> newlyPublishedContent, IOldPickedValuesProvider oldPickerValueProvider, IUmbracoServiceWrapper umbracoServiceWrapper)
        {
            //Ok so we have the newly published versions here. We need to get the old versions and compare.
            foreach (IContentBase newContent in newlyPublishedContent)
            {
                if (String.IsNullOrEmpty(oppositePickerPropertyAlias)) continue;
                UpdateOppositePicker(thisPickerPropertyAlias, oppositePickerPropertyAlias, newContent, oldPickerValueProvider, umbracoServiceWrapper);
            }
            return true;
        }


        public bool UpdateOppositePicker(string thisPickerPropertyAlias, string oppositePickerPropertyAlias, IContentBase newContent, IOldPickedValuesProvider oldPickerValueProvider, IUmbracoServiceWrapper umbracoServiceWrapper)
        {
            UmbracoHelper uh = new UmbracoHelper(UmbracoContext.Current);

            IEnumerable<int> oldPickedItems = oldPickerValueProvider.GetOldPickedItems(thisPickerPropertyAlias, newContent).ToList();
            IEnumerable<int> newPickedItems = newContent.GetValue<string>(thisPickerPropertyAlias).ToIntList();

            IEnumerable<int> deletedItems = oldPickedItems.Except(newPickedItems);
            IEnumerable<int> addedItems = newPickedItems.Except(oldPickedItems);

            //Now we want to go update the pickers. 
            UpdateItemsInOppositePicker("delete", newContent.Id, deletedItems, oppositePickerPropertyAlias, umbracoServiceWrapper);
            UpdateItemsInOppositePicker("add", newContent.Id, addedItems, oppositePickerPropertyAlias, umbracoServiceWrapper);

            return true;
        }


        private void UpdateItemsInOppositePicker(string addOrDelete, int currentItemId, IEnumerable<int> changedItems, string oppositePickerPropertyAlias, IUmbracoServiceWrapper itemService)
        {
            foreach (int changedItemId in changedItems)
            {
                IContentBase item = itemService.GetById(changedItemId);

                if (item == null) return;

                List<int> pickedItems = GetPickedItems(item, oppositePickerPropertyAlias).ToList();

                if (addOrDelete.InvariantEquals("add"))
                {
                    pickedItems.Add(currentItemId);
                }
                else
                {
                    pickedItems.Remove(currentItemId);
                }

                try
                {
                    //Update the opposite picker value with the now delete one.s
                    item.SetValue(oppositePickerPropertyAlias, string.Join(",", pickedItems.Distinct()));
                }
                catch{}
                


                itemService.Save(item, true, raiseEvents: false);

            }
        }


        public IEnumerable<int> GetPickedItems(IContentBase content, string propertyAlias)
        {
            if(content == null)
            {
                throw new ArgumentNullException("content");
            }

            try
            {
                IEnumerable<IPublishedContent> pickedContent = content.GetValue<IEnumerable<IPublishedContent>>(propertyAlias);

                //Interesting, if we have the Our Core Property Value Converters package installed, this will return null, so check to see if 
                //we can get it back as an IEnumerable<IPublishedContent>
                if (pickedContent == null || !pickedContent.Any())
                {
                    string pickedItems = content.GetValue<string>(propertyAlias);

                    if (String.IsNullOrEmpty(pickedItems))
                    {
                        return Enumerable.Empty<int>();
                    }
                    else
                    {
                        return pickedItems.ToIntList();
                    }
                }
                else
                {
                    return pickedContent.Select(x => x.Id);
                }
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Tried to GetPickedItems for content name {0} and property alias {1}", content.Name, propertyAlias));
                return Enumerable.Empty<int>();
            }   
        }


        public IEnumerable<int> GetPickedItems(IPublishedContent content, string propertyAlias)
        {
            IEnumerable<IPublishedContent> pickedContent = content.GetPropertyValue<IEnumerable<IPublishedContent>>(propertyAlias);

            //Interesting, if we have the Our Core Property Value Converters package installed, this will return null, so check to see if 
            //we can get it back as an IEnumerable<IPublishedContent>
            if (pickedContent == null || !pickedContent.Any())
            {
                string pickedItems = content.GetPropertyValue<string>(propertyAlias);

                if (String.IsNullOrEmpty(pickedItems))
                {
                    return Enumerable.Empty<int>();
                }
                else
                {
                    return pickedItems.ToIntList();
                }
            }
            else
            {
                return pickedContent.Select(x => x.Id);
            }
        }
    }
}