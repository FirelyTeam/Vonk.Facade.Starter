using Microsoft.EntityFrameworkCore;
using System;
using Vonk.Core.Common;
using Vonk.Facade.Relational;

namespace Visi.Repository
{
    public abstract class VisiQueryFactory<E, Q> : RelationalQueryFactory<E, Q> where E : class where Q : RelationalQuery<E>, new()
    {
        protected VisiQueryFactory(string forResourceType, DbContext onContext) : base(forResourceType, onContext)
        {
        }

        public override Q EntryInformationModel(string informationModel)
        {
            if (!VonkConstants.Model.FhirR3.Equals(informationModel))
                throw new NotSupportedException($"This facade only supports {VonkConstants.Model.FhirR3}, not {informationModel}");
            return default; //Since we only support 1 FHIR version, there is no need to provide an actual filter.
        }
    }
}
