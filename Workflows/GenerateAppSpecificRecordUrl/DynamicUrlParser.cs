﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAppSpecificRecordUrl {
    /// <summary> 
    /// Used to parse the Dynamics CRM 'Record URL (Dynamic)' that can be created by workflows and dialogues. 
    /// </summary> 
    public class DynamicUrlParser {
        public string Url {
            get; set;
        }
        public int EntityTypeCode {
            get; set;
        }
        public string BaseUrl {
            get; set;
        }
        public Guid Id {
            get; set;
        }

        /// <summary> 
        /// Parse the Dynamic URL in constructor 
        /// </summary> 
        /// <param name="url"></param> 
        public DynamicUrlParser(string url) {
            try {
                Url = url;
                var uri = new Uri(url);
                int found = 0;

                BaseUrl = url.Split('?').First();   // sorry

                string[] parameters = uri.Query.TrimStart('?').Split('&');
                foreach (string param in parameters) {
                    var nameValue = param.Split('=');
                    switch (nameValue[0]) {
                        case "etc":
                            EntityTypeCode = int.Parse(nameValue[1]);
                            found++;
                            break;
                        case "id":
                            Id = new Guid(nameValue[1]);
                            found++;
                            break;
                    }
                    if (found > 1)
                        break;
                }
            }
            catch (Exception ex) {
                throw new Exception(String.Format("URL '{0}' is incorrectly formatted for a Dynamics CRM Dynamics URL", url), ex);
            }
        }

        /// <summary> 
        /// Find the Logical Name from the entity type code - this needs a reference to the Organization Service to look up metadata 
        /// </summary> 
        /// <param name="service"></param> 
        /// <returns></returns> 
        public string GetEntityLogicalName(IOrganizationService service) {
            var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode ", MetadataConditionOperator.Equals, this.EntityTypeCode));
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
            propertyExpression.PropertyNames.Add("LogicalName");
            var entityQueryExpression = new EntityQueryExpression() {
                Criteria = entityFilter,
                Properties = propertyExpression
            };

            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest() {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse) service.Execute(retrieveMetadataChangesRequest);

            if (response.EntityMetadata.Count == 1) {
                return response.EntityMetadata[0].LogicalName;
            }
            return null;
        }


        public EntityReference GetEntityReference(IOrganizationService service) {
            var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode ", MetadataConditionOperator.Equals, this.EntityTypeCode));
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
            propertyExpression.PropertyNames.Add("LogicalName");
            var entityQueryExpression = new EntityQueryExpression() {
                Criteria = entityFilter,
                Properties = propertyExpression
            };

            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest() {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse) service.Execute(retrieveMetadataChangesRequest);

            if (response.EntityMetadata.Count == 1) {
                string logicalName = response.EntityMetadata[0].LogicalName;
                return new EntityReference(logicalName, Id);
            }
            return null;
        }
    }
}