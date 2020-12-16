using System;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using Visi.Repository.Models;
using Vonk.Core.Repository;
using Vonk.Core.Repository.ResultShaping;
using Vonk.Core.Support;
using Vonk.Facade.Relational;
using static Vonk.Core.Common.VonkConstants;

namespace Visi.Repository
{
    public class PatientQuery : RelationalQuery<ViSiPatient>
    {
        public PatientQuery() : base() { }

        public PatientQuery(SortShape sort)
        {
            _sort = sort;
        }
        private readonly SortShape _sort;
        public override IShapeValue[] Shapes => _sort is null ? base.Shapes :
            base.Shapes.SafeUnion(new[] { _sort }).ToArray();

        protected override IQueryable<ViSiPatient> HandleShapes(IQueryable<ViSiPatient> source)
        {
            var sorted = _sort is null ? source :
                (_sort.Direction == SortDirection.ascending ? source.OrderBy(vp => vp.Id) :
                source.OrderByDescending(vp => vp.Id));
            return base.HandleShapes(sorted);
        }
    }

    public class PatientQueryFactory : RelationalQueryFactory<ViSiPatient, PatientQuery>
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

        public override PatientQuery ResultShape(IShapeValue shape)
        {
            if (shape is SortShape sort && sort.ParameterName == "_lastUpdated")
            {
                return new PatientQuery(sort);
            }
            return base.ResultShape(shape);
        }
    }
}
