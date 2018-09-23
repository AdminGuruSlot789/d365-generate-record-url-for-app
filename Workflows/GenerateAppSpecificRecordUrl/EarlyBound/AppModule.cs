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
    public class AppModule : Entity {

        public const string EntityLogicalName = "appmodule";

        public class FieldNames {
            public const string Id = "appmoduleid";
            public const string ClientType = "clienttype";
            public const string Name = "name";
            public const string UniqueName = "uniquename";
            public const string Url = "url";
        }


        public class ClientTypes {
            public const int CLASSIC_INTERFACE = 2;
            public const int UNIFIED_INTERFACE = 4;
            public const int OUTLOOK_APP = 8;
        }


        public AppModule() : base(EntityLogicalName) { }


        /// <summary>
        ///     The user interface type of the app.
        /// </summary>
        [AttributeLogicalName(FieldNames.ClientType)]
        public int? ClientType {
            get {
                return Contains(FieldNames.ClientType) ? GetAttributeValue<int>(FieldNames.ClientType) : (int?)null;
            }
            set {
                SetAttributeValue(FieldNames.ClientType, value);
            }
        }


        /// <summary>
        ///     The name of the app.
        /// </summary>
        [AttributeLogicalName(FieldNames.Name)]
        public string Name {
            get => GetAttributeValue<string>(FieldNames.Name);
            set => SetAttributeValue(FieldNames.Name, value);
        }


        /// <summary>
        ///     The URL of the app.
        /// </summary>
        [AttributeLogicalName(FieldNames.Url)]
        public string Url {
            get => GetAttributeValue<string>(FieldNames.Url);
            set => SetAttributeValue(FieldNames.Url, value);
        }


        /// <summary>
        ///     The unique name of the app.
        /// </summary>
        [AttributeLogicalName(FieldNames.UniqueName)]
        public string UniqueName {
            get => GetAttributeValue<string>(FieldNames.UniqueName);
            set => SetAttributeValue(FieldNames.UniqueName, value);
        }
    }
}
