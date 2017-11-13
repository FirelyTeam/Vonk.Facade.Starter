using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vonk.Core.Repository;
using Vonk.Facade.Relational;
using Vonk.Facade.Starter.Models;

namespace Vonk.Facade.Starter.Repository
{
    public class PatientQuery : RelationalQuery<ViSiPatient>
    {
    }

    public class BloodPressureQuery : RelationalQuery<ViSiBloodPressure>
    {
    }

    public class PatientQueryFactory : RelationalQueryFactory<ViSiPatient, PatientQuery>
    {
        public PatientQueryFactory() : base("Patient") { }

        public override PatientQuery AddValueFilter(string parameterName, TokenValue value)
        {
            if (parameterName == "_id")
            {
                if (!long.TryParse(value.Code, out long patientId))
                {
                    throw new ArgumentException("Patient Id must be an integer value.");
                }
                else
                {
                    return PredicateQuery(vp => vp.Id == patientId);
                }
            }
            return base.AddValueFilter(parameterName, value);
        }

        public override PatientQuery AddValueFilter(string parameterName, ReferenceFromValue value)
        {
            if (parameterName == "subject" && value.Source == "Observation")
            {
                var obsQuery = value.CreateQuery(new BPQueryFactory());
                return PredicateQuery(p => p.BloodPressure.Any(obsQuery.Predicate.Compile()));
                //var observations = obsQuery.Execute()
                //return ChainQuery(obsQuery, (p, bps) => bps.Select(bp => bp.PatientId).Contains(p.Id));
            }
            return base.AddValueFilter(parameterName, value);
        }
    }

    public class BPQueryFactory : RelationalQueryFactory<ViSiBloodPressure, BloodPressureQuery>
    {
        public BPQueryFactory() : base("Observation") { }

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
    }
}
