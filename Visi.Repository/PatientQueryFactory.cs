using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Visi.Repository.Models;
using Vonk.Core.Repository;
using Vonk.Facade.Relational;

namespace Visi.Repository
{
    public class PatientQuery : RelationalQuery<ViSiPatient>
    {
    }

    public class PatientQueryFactory : RelationalQueryFactory<ViSiPatient, PatientQuery>
    {
        public PatientQueryFactory(DbContext onContext) : base("Patient", onContext) { }

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
            else if (parameterName == "identifier")
            {
                return PredicateQuery(vp => vp.PatientNumber == value.Code);
            }
            return base.AddValueFilter(parameterName, value);
        }

        public override PatientQuery AddValueFilter(string parameterName, ReferenceFromValue value)
        {
            if (parameterName == "subject" && value.Source == "Observation")
            {
                var obsQuery = value.CreateQuery(new BPQueryFactory(OnContext));
                var obsIds = obsQuery.Execute(OnContext).Select(bp => bp.PatientId);

                return PredicateQuery(p => obsIds.Contains(p.Id));
            }
            return base.AddValueFilter(parameterName, value);
        }
    }
}
