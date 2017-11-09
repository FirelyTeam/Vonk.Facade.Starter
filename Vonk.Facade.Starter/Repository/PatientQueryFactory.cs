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
    }
}
