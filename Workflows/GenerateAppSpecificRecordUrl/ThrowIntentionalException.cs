using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAppSpecificRecordUrl {

    /// <summary>
    ///     Throws an exception intentionally.
    ///     The intended use of this is to produce a URL that can be copied and pasted into a new
    ///     browser window.
    /// </summary>
    public class ThrowIntentionalException : CodeActivity {

        [Input("Message to display")]
        public InArgument<string> Message {
            get;set;
        }

        protected override void Execute(CodeActivityContext context) {
            ITracingService tracingService = context.GetExtension<ITracingService>();
            string message = Message.Get(context);

            tracingService.Trace(message);

            throw new InvalidWorkflowException("\n\n" + message + "\n\n");
        }
    }
}
