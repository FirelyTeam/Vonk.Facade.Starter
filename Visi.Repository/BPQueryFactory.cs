using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Vonk.Core.Repository;
using Vonk.Facade.Relational;
using Visi.Repository.Models;
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
