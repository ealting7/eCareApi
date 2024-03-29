﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class AssessItem
    {
        public int inpatientAdmissionId { get; set; }

        public int basicGeneralAssessId { get; set; }

        public bool? isReAassessment { get; set; }

        public string goalText { get; set; }
        public string planText { get; set; }

        //GENERAL ASSESS
        public int? breathingRateId { get; set; }
        public string? breathingRate { get; set; }
        public int? breathingTypeId { get; set; }
        public string? breathingType { get; set; }
        public int? chronologicalDevelopmentAppearanceId { get; set; }
        public string? developmentAppearance { get; set; }
        public int? mentalStatusId { get; set; }
        public string? mentalStatus { get; set; }
        public int? painLevelId { get; set; }
        public string? painLevel { get; set; }
        public int? alertnessStateId { get; set; }
        public string? alertnessName { get; set; }
        public bool? answeredNameCorrectly { get; set; }
        public bool? answeredDobCorrectly { get; set; }
        public int? hospitalChronologicalDevelopmentAppearanceId { get; set; }
        public bool? stateOfHealthThin { get; set; }
        public bool? stateOfHealthCachectic { get; set; }
        public bool? stateOfHealthTemporalWasting { get; set; }
        public bool? stateOfHealthPale { get; set; }
        public bool? stateOfHealthDiaphoretic { get; set; }
        public bool? stateOfHealthSignsOfPain { get; set; }
        public bool? complainsOfDiscomfort { get; set; }
        public bool? signsOfDiscomfort { get; set; }
        public string? painDetails { get; set; }
        public int? hospitalReassessmentTimeframeId { get; set; }
        public string? assessmentResult { get; set; }
        public bool? requestMedication { get; set; }
        public bool? needsWoundCare { get; set; }
        public bool? requestLab { get; set; }
        public bool? needsSocialServie { get; set; }
        public bool? requestTherapy { get; set; }
        public bool? callDr { get; set; }


        //VITALS
        public int? pulseIntensityId { get; set; }
        public string? pulseIntensity { get; set; }
        public int? temperatureSiteId { get; set; }
        public string? temperatureSite { get; set; }
        public int? respirationRegularityId { get; set; }
        public string? respirationRegularity { get; set; }
        public int? respirationDepthId { get; set; }
        public string? respirationDepth { get; set; }
        public int? pulsePositionForReadingId { get; set; }
        public string? pulsePositionForReading { get; set; }
        public int? pulseRhythmId { get; set; }
        public string? pulseRhythm { get; set; }


        //HEENT HEAD
        public int? headSkinColorId { get; set; }
        public string? skinColor { get; set; }
        public int? headProportionToBodyId { get; set; }
        public string? proportionSize { get; set; }


        //ABDOMEN
        public int? abdominalContourId { get; set; }
        public string? abdominalContour { get; set; }


        //EXTREMITES UPPER
        public int? upperPalpateArteryStrengthId { get; set; }
        public string? upperPalpateArteryStrength { get; set; }
        public int? upperSqueezePushStrengthId { get; set; }
        public string? upperSqueezePushStrength { get; set; }



        //RESPIRATORY
        public bool? auscultationCrackling { get; set; }
        public bool? auscultationWheezing { get; set; }
        public bool? auscultationRhonchi { get; set; }
        public bool? auscultationStridor { get; set; }
        public bool? auscultationWhooping { get; set; }
        public bool? coughPresent { get; set; }
        public bool? coughProductive { get; set; }
        public bool? gagReflexNormal { get; set; }
        public int? intubationMethodId { get; set; }

        public int? ettTubeTypeId { get; set; }
        public decimal? ettTubeSize { get; set; }
        public string ettTubeCare { get; set; }
        public decimal ettTubeCuffPressure { get; set; }

        public int? trachealSuctionMethodId { get; set; }

        public bool? sputumThick { get; set; }
        public bool? sputumYellowGreen { get; set; }
        public bool? sputumBloody { get; set; }
        public bool? sputumFrothyPink { get; set; }
        public bool? sputumBrown { get; set; }

        public decimal? subglotticTubeSuctionPressure { get; set; }
        public decimal? subglotticTubeSuctionAmount { get; set; }
        public string subglotticTubeSuctionDescription { get; set; }

        public int? ventilationTypeId { get; set; }
        public int? ventilationModeId { get; set; }
        public int? ventilatorRespiratoryRate { get; set; }
        public decimal? ventilatorInspiratoryPressure { get; set; }
        public decimal? ventilatorPressureSupport { get; set; }
        public decimal? ventilatorPeep { get; set; }
        public decimal? ventilatorTidalVolume { get; set; }
        public decimal? ventilatorPeakInspiratoryPressure { get; set; }
        public decimal? ventilatorInspiratoryExpiratoryPressure { get; set; }
        public decimal? ventilatorFi02 { get; set; }
        public decimal? ventilatorInspiratoryTime { get; set; }
        









        //OUTCOME FREQUENCY
        public int outcomeGoalId { get; set; }
        public string? outcomeGoalMeasure { get; set; }



        //INTERVENTION
        public int interventionFrequencyAdministrationId { get; set; }
        public string? interventionAdministrationFrequency { get; set; }
        public string? interventionFrequencyAbbrev { get; set; }
        public int interventionTypeId { get; set; }
        public string interventionType { get; set; }


        //DIAGNOSIS
        public int diagnosisDomainId { get; set; }
        public string diagnosisDomainName { get; set; }
        public int diagnosisClassId { get; set; }
        public int diagnosisClassDomainId { get; set; }
        public string diagnosisClassName { get; set; }
        public int diagnosisPriorityId { get; set; }
        public string? diagnosisPriorityDescription {get;set;}



        public string? sourceLoaderDescription { get; set; }
        public int sourceLoaderId { get; set; }
        public string sourceLoaderTextId { get; set; }
    }
}
