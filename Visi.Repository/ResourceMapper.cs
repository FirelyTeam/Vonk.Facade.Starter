using Hl7.Fhir.Model;
using Vonk.Core.Common;
using Vonk.Core.Context;
using Visi.Repository.Models;
using Vonk.Fhir.R3;

namespace Visi.Repository
{
    public class ResourceMapper
    {
        public IResource MapPatient(ViSiPatient source)
        {
            var patient = new Patient
            {
                Id = source.Id.ToString(),
                BirthDate = new FhirDateTime(source.DateOfBirth).ToString()
            };
            patient.Identifier.Add(new Identifier("http://mycompany.org/patientnumber", source.PatientNumber));
            patient.Name.Add(new HumanName().WithGiven(source.FirstName).AndFamily(source.FamilyName));
            patient.Telecom.Add(new ContactPoint(ContactPoint.ContactPointSystem.Email, ContactPoint.ContactPointUse.Home, source.EmailAddress));
            return patient.AsIResource();
        }

        public IResource MapBloodPressure(ViSiBloodPressure bp)
        {
            var observation = new Observation
            {
                Id = bp.Id.ToString(),
                Subject = new ResourceReference($"Patient/{bp.PatientId}"),
                Effective = new FhirDateTime(bp.MeasuredAt),
                Status = ObservationStatus.Final
            };
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
