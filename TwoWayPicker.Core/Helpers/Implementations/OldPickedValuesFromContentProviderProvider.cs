using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoWayPicker.Core.Helpers.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Web;
using TwoWayPicker.Core.Extensions;

namespace TwoWayPicker.Core.Helpers.Implementations
{
    public class OldPickedValuesFromContentProviderProvider : IOldPickedValuesProvider
    {
        private UmbracoHelper _umbracoHelper;
        public OldPickedValuesFromContentProviderProvider(UmbracoHelper uh)
        {
            _umbracoHelper = uh;
        }

        public IEnumerable<int> GetOldPickedItems(string thisPickerPropertyAlias, IContentBase newContent)
        {
            IPublishedContent oldContent = _umbracoHelper.TypedContent(newContent.Id);

            if (oldContent == null)
            {
                //this must be a new item. so there are no old picked items.
                return Enumerable.Empty<int>();
            }

            IEnumerable<int> pickedItems = GetPickedItems(oldContent, thisPickerPropertyAlias);

            return pickedItems;
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
