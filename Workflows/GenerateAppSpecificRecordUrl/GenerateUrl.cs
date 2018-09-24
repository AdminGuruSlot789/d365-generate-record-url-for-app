using GenerateAppSpecificRecordUrl.EarlyBound;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;

namespace GenerateAppSpecificRecordUrl {

    /// <summary>
    ///     Generates a Record URL based on the provided Record URL and the App specified in a
    ///     configuration record.
    /// </summary>
    public class GenerateUrl : CodeActivity {

        private class PageTypes {
            public const string ENTITY_LIST = "entitylist";
            public const string ENTITY_RECORD = "entityrecord";
        }

        [Input("Record URL (Dynamic)")]
        [RequiredArgument]
        public InArgument<string> RecordUrl {
            get; set;
        }
        
        [Input("App Name")]
        [RequiredArgument]
        public InArgument<string> AppName {
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

            // Read InArguments.
            string recordUrl = RecordUrl.Get<string>(codeActivityContext);
            string appName = AppName.Get(codeActivityContext);

            // Create URL.
            string newRecordUrl = GenerateUrlForApp(service, recordUrl, appName);

            // Set OutArgument.
            NewRecordUrl.Set(codeActivityContext, newRecordUrl);
        }


        public static string GenerateUrlForApp(IOrganizationService service, string recordUrl, string appName) {
            // Get base URL of the instance.
            string baseUrl = recordUrl.Split('?')[0];

            // Get the entity reference from the Dynamic Record URL passed to the CodeActivity.
            DynamicUrlParser dynamicUrlParser = new DynamicUrlParser(recordUrl);
            EntityReference entityReference = dynamicUrlParser.GetEntityReference(service);

            // Query for AppModule record.
            AppModule appModule = GetAppModule(service, appName);

            // Create URL.
            return CreateRecordUrl(baseUrl, appModule, entityReference, dynamicUrlParser);
        }


        public static AppModule GetAppModule(IOrganizationService service, string appName) {
            string fetchXml = $@"
                <fetch>
                    <entity name='{AppModule.EntityLogicalName}'>
                        <attribute name='{AppModule.FieldNames.Id}' />
                        <attribute name='{AppModule.FieldNames.ClientType}' />
                        <attribute name='{AppModule.FieldNames.Name}' />
                        <attribute name='{AppModule.FieldNames.UniqueName}' />
                        <attribute name='{AppModule.FieldNames.Url}' />
                        <filter type='or'>
                            <condition attribute='{AppModule.FieldNames.Name}'
                                       operator='{FetchXmlOperators.Like}'
                                       value='{appName}' />
                            <condition attribute='{AppModule.FieldNames.UniqueName}'
                                       operator='{FetchXmlOperators.Like}'
                                       value='{appName}' />
                        </filter>
                    </entity>
                </fetch>".RemoveExtraWhiteSpaceInFetchXml();

            AppModule appModule = (AppModule)service.RetrieveMultiple(new FetchExpression(fetchXml)).Entities.FirstOrDefault();

            if (appModule == default(AppModule)) {
                throw new InvalidWorkflowException($"No App was found with the name: {appName}.");
            }

            return appModule;
        }


        public static string CreateRecordUrl(string baseUrl, AppModule appModule, EntityReference entityReference, DynamicUrlParser dynamicUrlParser) {
            // Compose the URL with the query string parameters needed to access the Unified Interface apps.
            string queryString = string.Empty;

            if (appModule.ClientType == AppModule.ClientTypes.UNIFIED_INTERFACE) {
                queryString = string.Join("&", new string[] {
                    "appid=" + appModule.Id.ToString("D"),
                    "pagetype=" + PageTypes.ENTITY_RECORD,
                    "etn=" + entityReference.LogicalName,
                    "id=" + entityReference.Id.ToString("D")
                });
            }
            else if (appModule.ClientType == AppModule.ClientTypes.CLASSIC_INTERFACE) {
                queryString = string.Join("&", new string[] {
                    "appid=" + appModule.Id.ToString("D"),
                    "etc=" + dynamicUrlParser.EntityTypeCode,
                    "id=" + entityReference.Id.ToString("D"),
                    "newWindow=true",
                    "pagetype=" + PageTypes.ENTITY_RECORD
                });
            }
            
            return baseUrl + "?" + queryString;
        }
        
    }
}
