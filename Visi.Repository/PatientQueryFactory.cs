using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Visi.Repository.Models;
using Vonk.Core.Repository;
using Vonk.Core.Repository.ResultShaping;
using Vonk.Facade.Relational;
using static Vonk.Core.Common.VonkConstants;

namespace Visi.Repository
{
    public class PatientQuery : RelationalQuery<ViSiPatient>
    {
    }

    public class PatientQueryFactory : VisiQueryFactory<ViSiPatient, PatientQuery>
    {
        public PatientQueryFactory(DbContext onContext) : base(nameof(Patient), onContext) { }

        public override PatientQuery AddValueFilter(string parameterName, TokenValue value)
        {
            if (parameterName == ParameterNames.Id)
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
            if (parameterName == "subject" && value.Source == nameof(Observation))
            {
                var obsQuery = value.CreateQuery(new BPQueryFactory(OnContext));
                var obsIds = obsQuery.Execute(OnContext).Select(bp => bp.PatientId);

                return PredicateQuery(p => obsIds.Contains(p.Id));
            }
            return base.AddValueFilter(parameterName, value);
        }

        public override PatientQuery AddValueFilter(string parameterName, MissingValue value)
        {
            var isMissing = value.IsMissing; // true

            switch (parameterName)
            {
                //_id is a bit contrived, as Id is never null in Visi, so if isMissing = true, return false, otherwise return true for every record. 
                case "_id": return PredicateQuery(p => !isMissing);
                //This is a more real example:
                case "identifier": return PredicateQuery(p => (p.PatientNumber == null) == isMissing); 
                default:
                    throw new ArgumentException($"Filtering for missing values using parameter {parameterName} is not supported.");
            }
        }

        protected override PatientQuery AddResultShape(SortShape sort)
        {
            switch (sort.ParameterName)
            {
                case "_id": return SortQuery(sort, p => p.Id);
                case "identifier": return SortQuery(sort, p => p.PatientNumber);
                default:
                    throw new ArgumentException($"Sorting on {sort.ParameterName} is not supported.");
            }
        }
    }
}
