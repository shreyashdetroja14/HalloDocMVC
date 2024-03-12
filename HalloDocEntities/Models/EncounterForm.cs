using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocEntities.Models;

[Table("encounter_form")]
public partial class EncounterForm
{
    [Key]
    [Column("encounter_form_id")]
    public int EncounterFormId { get; set; }

    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Column("location")]
    [StringLength(200)]
    public string? Location { get; set; }

    [Column("str_month")]
    [StringLength(20)]
    public string? StrMonth { get; set; }

    [Column("int_year")]
    public int? IntYear { get; set; }

    [Column("int_date")]
    public int? IntDate { get; set; }

    [Column("service_date", TypeName = "timestamp without time zone")]
    public DateTime? ServiceDate { get; set; }

    [Column("phone_number")]
    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("present_illness_history")]
    [StringLength(500)]
    public string? PresentIllnessHistory { get; set; }

    [Column("medical_history")]
    [StringLength(500)]
    public string? MedicalHistory { get; set; }

    [Column("medications")]
    [StringLength(500)]
    public string? Medications { get; set; }

    [Column("allergies")]
    [StringLength(500)]
    public string? Allergies { get; set; }

    [Column("temperature")]
    [StringLength(50)]
    public string? Temperature { get; set; }

    [Column("heart_rate")]
    [StringLength(50)]
    public string? HeartRate { get; set; }

    [Column("respiration_rate")]
    [StringLength(50)]
    public string? RespirationRate { get; set; }

    [Column("blood_pressure_systolic")]
    [StringLength(50)]
    public string? BloodPressureSystolic { get; set; }

    [Column("blood_pressure_diastolic")]
    [StringLength(50)]
    public string? BloodPressureDiastolic { get; set; }

    [Column("oxygen_level")]
    [StringLength(50)]
    public string? OxygenLevel { get; set; }

    [Column("pain")]
    [StringLength(50)]
    public string? Pain { get; set; }

    [Column("heent")]
    [StringLength(500)]
    public string? Heent { get; set; }

    [Column("cardiovascular")]
    [StringLength(500)]
    public string? Cardiovascular { get; set; }

    [Column("chest")]
    [StringLength(500)]
    public string? Chest { get; set; }

    [Column("abdomen")]
    [StringLength(500)]
    public string? Abdomen { get; set; }

    [Column("extremities")]
    [StringLength(500)]
    public string? Extremities { get; set; }

    [Column("skin")]
    [StringLength(500)]
    public string? Skin { get; set; }

    [Column("neuro")]
    [StringLength(500)]
    public string? Neuro { get; set; }

    [Column("other")]
    [StringLength(500)]
    public string? Other { get; set; }

    [Column("diagnosis", TypeName = "character varying")]
    public string? Diagnosis { get; set; }

    [Column("treatment_plan", TypeName = "character varying")]
    public string? TreatmentPlan { get; set; }

    [Column("medications_dispensed", TypeName = "character varying")]
    public string? MedicationsDispensed { get; set; }

    [Column("procedures", TypeName = "character varying")]
    public string? Procedures { get; set; }

    [Column("follow_up", TypeName = "character varying")]
    public string? FollowUp { get; set; }

    [Column("created_date", TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_date", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [Column("is_finalized")]
    public bool? IsFinalized { get; set; }

    [Column("finalized_date", TypeName = "timestamp without time zone")]
    public DateTime? FinalizedDate { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("EncounterForms")]
    public virtual Request Request { get; set; } = null!;
}
