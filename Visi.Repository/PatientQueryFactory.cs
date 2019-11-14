using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
            if (Shapes is null)
                return source;

            var sorted = source;
            foreach (var sortShape in Shapes.OfType<SortShape>())
            {
                if (sortShape.ParameterName == "_id")
                    sorted = sortShape.Direction == SortDirection.ascending ? sorted.OrderBy(vp => vp.Id) : sorted.OrderByDescending(vp => vp.Id);
                else if (sortShape.ParameterName == "identifier")
                    sorted = sortShape.Direction == SortDirection.ascending ? sorted.OrderBy(vp => vp.PatientNumber) : sorted.OrderByDescending(vp => vp.PatientNumber);
            }
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

        private readonly static string[] _supportedSortFields = new[] { "_id", "identifier" };
        public override PatientQuery ResultShape(IShapeValue shape)
        {
            if (shape is SortShape sort)
            {
                if (!_supportedSortFields.Contains(sort.ParameterName))
                {
                    throw new ArgumentException($"Sorting on {sort.ParameterName} is not supported.");
                }
                return new PatientQuery(sort);
            }
            return base.ResultShape(shape);
        }
    }
}
