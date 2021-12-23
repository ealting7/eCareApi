using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class CareplanAssessItem
    {
        public List<AssessItem> breathingRates { get; set; }
        public List<AssessItem> chronologicalDevelopmentAppearances { get; set; }
        public List<AssessItem> breathingTypes { get; set; }
        public List<AssessItem> mentalStatuses { get; set; }
        public List<AssessItem> painLevels { get; set; }
        public List<AssessItem> alertnessStates { get; set; }

        public List<AssessItem> temperatureSites { get; set; }
        public List<AssessItem> respirationRegularity { get; set; }
        public List<AssessItem> respirationDepth { get; set; }
        public List<AssessItem> pulsePosture { get; set; }
        public List<AssessItem> pulseRhythm { get; set; }
        public List<AssessItem> pulseIntensity { get; set; }

        public List<AssessItem> headSkinColor { get; set; }
        public List<AssessItem> headProportionToBody { get; set; }

        public List<AssessItem> abdomenAbdominalContour { get; set; }

        public List<AssessItem> upperPalpateArteryStrength { get; set; }
        public List<AssessItem> upperSqueezePushStrength { get; set; }        


        public List<AssessItem> diagnosisDomains { get; set; }
        public List<AssessItem> diagnosisClass { get; set; }
        public List<AssessItem> diagnosisPriority { get; set; }


        public List<AssessItem> outcomeGoalMeasurment { get; set; }



        public List<AssessItem> interventionFrequency { get; set; }
        public List<AssessItem> interventionTypes { get; set; }
    }
}
