using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using Visi.Repository.Models;
using Vonk.Core.Repository;
using Vonk.Core.Repository.ResultShaping;
using Vonk.Core.Support;
using Vonk.Facade.Relational;
using static Vonk.Core.Common.VonkConstants;

namespace Visi.Repository
{
    public delegate IQueryable<ViSiPatient> Sort(IQueryable<ViSiPatient> input);

    public class PatientQuery : RelationalQuery<ViSiPatient>
    {
        public PatientQuery() : base() { }

        public PatientQuery(Sort sort) : this()
        {
            SortOperations = new[] { sort };
        }

        protected override IQueryable<ViSiPatient> HandleShapes(IQueryable<ViSiPatient> source)
        {
            if (Shapes is null)
                return source;

            var sorted = source;
            if (SortOperations.HasAny())
            {
                foreach (var sortOp in SortOperations)
                {
                    sorted = sortOp(sorted);
                }
            }
            return base.HandleShapes(sorted);
        }

        internal Sort[] SortOperations { get; set; }
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
            if (shape is SortShape sort)
            {
                switch (sort.ParameterName)
                {
                    case "_id": return new PatientQuery(input => input.Sort(sort.Direction, (ViSiPatient p) => p.Id));
                    case "identifier": return new PatientQuery(input => input.Sort(sort.Direction, (ViSiPatient p) => p.PatientNumber));
                    default:
                        throw new ArgumentException($"Sorting on {sort.ParameterName} is not supported.");
                }
            }
            return base.ResultShape(shape);
        }
        public override PatientQuery And(PatientQuery left, PatientQuery right)
        {
            var result = base.And(left, right);
            if (result is object)
            {
                result.SortOperations =
                    left is null ? right?.SortOperations
                        : (right is null ? left?.SortOperations : left.SortOperations.SafeUnion(right.SortOperations)?.ToArray());
            }
            return result;
        }
    }

    public static class SortExtensions
    {
        public static IQueryable<TSource> Sort<TSource, TKey>(this IQueryable<TSource> source, SortDirection direction, Expression<Func<TSource, TKey>> field)
        {
            return direction == SortDirection.ascending ? source.OrderBy(field) : source.OrderByDescending(field);
        }
    }
}
