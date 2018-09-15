using GenerateAppSpecificRecordUrl.EarlyBound;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace GenerateAppSpecificRecordUrl {

    /// <summary>
    ///     Generates a Record URL based on the provided Record URL and the App specified in a
    ///     configuration record.
    /// </summary>
    public class GenerateUrl : CodeActivity {

        private const string SETTINGS_ENTITY_NAME = "codec_setting";
        private const string SETTINGS_ENTITY_VALUE_FIELD_NAME = "codec_name";

        private const string PAGE_TYPE = "entityrecord";


        [Input("Record URL (Dynamic)")]
        [RequiredArgument]
        public InArgument<string> RecordUrl {
            get; set;
        }

        [Input("App Id Setting Reference")]
        [ReferenceTarget(CustomSetting.EntityLogicalName)]
        public InArgument<EntityReference> AppIdSettingReference {
            get; set;
        }


        [Output("New record URL")]
        public OutArgument<string> NewRecordUrl {
            get; set;
        }


        protected override void Execute(CodeActivityContext codeActivityContext) {
            // Set up ITracingService, IOrganizationService.
            ITracingService tracingService = codeActivityContext.GetExtension<ITracingService>();
            IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // Get Record URL (Dynamic).
            // Get base URL of the instance.
            string recordUrl = RecordUrl.Get<string>(codeActivityContext);
            DynamicUrlParser dynamicUrlParser = new DynamicUrlParser(recordUrl);

            EntityReference entityReference = dynamicUrlParser.GetEntityReference(service);

            // Get specified settings record.
            // Get appId from settings record.
            // Parse the appId to make sure it is valid.
            EntityReference appIdSettingReference = AppIdSettingReference.Get<EntityReference>(codeActivityContext);

            Guid appId = GetAppId(service, appIdSettingReference);

            // Create URL.
            string newRecordUrl = CreateRecordUrl(dynamicUrlParser.BaseUrl, appId, entityReference);

            // Set OutArgument.
            NewRecordUrl.Set(codeActivityContext, newRecordUrl);
        }

        public static Guid GetAppId(IOrganizationService service, EntityReference appIdSettingReference) {
            //string appIdString = service.Retrieve(appIdSettingReference.LogicalName, appIdSettingReference.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(Settings.FieldNames.Value)).GetAttributeValue<string>(Settings.FieldNames.Value);
            //Entity setting = service.Retrieve(appIdSettingReference.LogicalName, appIdSettingReference.Id, new ColumnSet(SETTINGS_ENTITY_VALUE_FIELD_NAME));
            //string appIdString = setting.GetAttributeValue<string>(SETTINGS_ENTITY_VALUE_FIELD_NAME);
            //Guid appId;
            //try {
            //    appId = Guid.Parse(appIdString);
            //}
            //catch {
            //    throw new Exception($"{nameof(appId)} is not assigned with a valid GUID.");
            //}
            //return appId;

            CustomSetting setting = service.Retrieve(appIdSettingReference.LogicalName, appIdSettingReference.Id, new ColumnSet(CustomSetting.FieldNames.Value)) as CustomSetting;
            Guid appId;
            try {
                appId = Guid.Parse(setting.Value.Trim());
            }
            catch {
                throw new Exception($"{nameof(appId)} is not assigned with a valid GUID.");
            }
            return appId;
        }

        public static string CreateRecordUrl(string baseUrl, Guid appId, EntityReference entityReference) {

            if (string.IsNullOrWhiteSpace(baseUrl)) {
                throw new Exception($"{nameof(baseUrl)} was not assigned.");
            }
            if (appId == default(Guid)) {
                throw new Exception($"{nameof(appId)} was not assigned.");
            }
            if (entityReference == default(EntityReference)) {
                throw new Exception($"{nameof(entityReference)} was not assigned.");
            }

            // Add to the properties that are already there.
            return $"{baseUrl}?appid={appId.ToString("D")}&pagetype={PAGE_TYPE}&etn={entityReference.LogicalName}&id={entityReference.Id.ToString("D")}";
        }
        
    }
}
