using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoWayPicker.Models
{
    public class TwoWayPickerPrevalueModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("propertyAliases")]
        public IEnumerable<string> PropertyAliases { get; set; }

        [JsonProperty("selectedProperty")]
        public string SelectedProperty { get; set; }
    }
}