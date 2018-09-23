using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAppSpecificRecordUrl {
    public class FetchXmlOperators {
        public const string Equals = "eq";
        public const string NotEqual = "ne";
        public const string Like = "like";
        public const string NotLike = "not-like";
        public const string In = "in";
        public const string NotIn = "not-in";
        public const string Null = "null";
        public const string NotNull = "not-null";
        public const string BeginsWith = "begins-with";
        public const string DoesNotBeginWith = "not-begin-with";
        public const string EndsWith = "ends-with";
        public const string DoesNotEndWith = "not-end-with";
        public const string ContainValues = "contain-values";
        public const string DoesNotContainValues = "not-contain-values";
    }
}
