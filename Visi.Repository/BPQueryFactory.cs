using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Visi.Repository.Models;
using Vonk.Core.Repository;
using Vonk.Core.Support;
using Vonk.Facade.Relational;

namespace Visi.Repository
{
    public class BloodPressureQuery : RelationalQuery<ViSiBloodPressure>
    {
    }
    public class BPQueryFactory : RelationalQueryFactory<ViSiBloodPressure, BloodPressureQuery>
    {
        public BPQueryFactory(DbContext onContext) : base("Observation", onContext) { }

        public override BloodPressureQuery AddValueFilter(string parameterName, TokenValue value)
        {
            if (parameterName == "_id")
            {
                if (!long.TryParse(value.Code, out long bpId))
                {
                    throw new ArgumentException("BloodPressure Id must be an integer value.");
                }
                else
                {
                    return PredicateQuery(vp => vp.Id == bpId);
                }
            }
            return base.AddValueFilter(parameterName, value);
        }

        public override BloodPressureQuery AddValueFilter(string parameterName, ReferenceValue value)
        {
            if (parameterName == "subject")
            {
                var patIdValue = value.Reference.StripFromStart("Patient/");
                if (!int.TryParse(patIdValue, out var patId))
                {
                    throw new ArgumentException("Patient Id must be an integer value");
                }
                return PredicateQuery(bp => bp.PatientId == patId);
            }
            return base.AddValueFilter(parameterName, value);
        }

        public override BloodPressureQuery AddValueFilter(string parameterName, ReferenceToValue value)
        {
            if (parameterName == "subject" && value.Targets.Contains("Patient"))
            {
                var patientQuery = value.CreateQuery(new PatientQueryFactory(OnContext));
                var patIds = patientQuery.Execute(OnContext).Select(p => p.Id);

                return PredicateQuery(bp => patIds.Contains(bp.PatientId));
            }
            return base.AddValueFilter(parameterName, value);
        }
    }
}
