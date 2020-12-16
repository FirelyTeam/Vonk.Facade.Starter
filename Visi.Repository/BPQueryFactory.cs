using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Visi.Repository.Models;
using Vonk.Core.Repository;
using Vonk.Core.Repository.ResultShaping;
using Vonk.Core.Support;
using Vonk.Facade.Relational;

namespace Visi.Repository
{
    public class BloodPressureQuery : RelationalQuery<ViSiBloodPressure>
    {
        public BloodPressureQuery() : base() { }

        public BloodPressureQuery(SortShape sort)
        {
            _sort = sort;
        }
        private readonly SortShape _sort;
        public override IShapeValue[] Shapes => _sort is null ? base.Shapes :
            base.Shapes.SafeUnion(new[] { _sort }).ToArray();

        protected override IQueryable<ViSiBloodPressure> HandleShapes(IQueryable<ViSiBloodPressure> source)
        {
            var sorted = _sort is null ? source :
                (_sort.Direction == SortDirection.ascending ? source.OrderBy(vp => vp.Id) :
                source.OrderByDescending(vp => vp.Id));
            return base.HandleShapes(sorted);
        }
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

        public override BloodPressureQuery ResultShape(IShapeValue shape)
        {
            if (shape is SortShape sort && sort.ParameterName == "_lastUpdated")
                return new BloodPressureQuery(sort);

            return base.ResultShape(shape);
        }
    }
}
