using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAppSpecificRecordUrl.EarlyBound {

    [DataContract]
    [EntityLogicalName(EntityLogicalName)]
    public class CustomSetting : Entity {

        public const string EntityLogicalName = "codec_setting";

        public class FieldNames {
            public const string Name = "codec_name";
            public const string Value = "codec_value";
        }


        public CustomSetting() : base(EntityLogicalName) { }


        /// <summary>
        ///     The name of the custom setting record.
        /// </summary>
        [AttributeLogicalName(FieldNames.Name)]
        public string Name {
            get => GetAttributeValue<string>(FieldNames.Name);
            set => SetAttributeValue(FieldNames.Name, value);
        }


        /// <summary>
        ///     The value of the custom setting record.
        /// </summary>
        [AttributeLogicalName(FieldNames.Value)]
        public string Value {
            get => GetAttributeValue<string>(FieldNames.Value);
            set => SetAttributeValue(FieldNames.Value, value);
        }
    }
}
