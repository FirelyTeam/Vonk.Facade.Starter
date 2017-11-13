using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vonk.Core.Common;
using Vonk.Core.Context;
using Vonk.Facade.Starter.Models;
using Vonk.Fhir.R3;

namespace Vonk.Facade.Starter.Repository
{
    public class ResourceMapper
    {
        public IResource MapPatient(ViSiPatient source)
        {
            var patient = new Patient();
            patient.Id = source.Id.ToString();
            patient.Identifier.Add(new Identifier("http://mycompany.org/patientnumber", source.PatientNumber));
            patient.Name.Add(new HumanName().WithGiven(source.FirstName).AndFamily(source.FamilyName));
            patient.BirthDate = new FhirDateTime(source.DateOfBirth).ToString();
            patient.Telecom.Add(new ContactPoint(ContactPoint.ContactPointSystem.Email, ContactPoint.ContactPointUse.Home, source.EmailAddress));
            return patient.AsIResource();
        }

        public IResource MapBloodPressure(ViSiBloodPressure bp)
        {
            var observation = new Observation();
            observation.Id = bp.Id.ToString();
            observation.Subject = new ResourceReference($"Patient/{bp.PatientId}");
            observation.Effective = new FhirDateTime(bp.MeasuredAt);
            observation.Status = ObservationStatus.Final;
            observation.Category.Add(new CodeableConcept("http://hl7.org/fhir/observation-category", "vital-signs", "Vital Signs"));
            observation.Component.Add(
                new Observation.ComponentComponent()
                {
                    Code = new CodeableConcept("http://loinc.org", "8480-6", "Systolic blood pressure"),
                    Value = new Quantity(bp.Systolic, "mm[Hg]", VonkConstants.UcumSystem)
                });
            observation.Component.Add(
                new Observation.ComponentComponent()
                {
                    Code = new CodeableConcept("http://loinc.org", "8462-4", "Diastolic blood pressure"),
                    Value = new Quantity(bp.Diastolic, "mm[Hg]", VonkConstants.UcumSystem)
                });

            return observation.AsIResource();
        }
    }
}
