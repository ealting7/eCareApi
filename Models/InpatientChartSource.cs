using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class InpatientChartSource
    {
        public int sourceId { get; set; }
        public string sourceName { get; set; }
        public int chartId { get; set; }
        public string chartName { get; set; }
        public string chartTableName { get; set; }
        public int? chartTableId { get; set; }
        public Guid? chartTableGuid { get; set; }
        public string chartType { get; set; }
        public string? sourceNameAbbrev { get; set; }
        public int? sourceOrder { get; set; }
        public string controlType { get; set; }
        public string controlClass { get; set; }
        public string modelVariableName { get; set; }
        public string modelDataType { get; set; }
        public string loaderTableName { get; set; }
        public string loaderIdColumnName { get; set; }
        public string loaderDescriptionColumnName { get; set; }
        public string? controlPattern { get; set; }
        public string? placeholder { get; set; }
        public int? maxLength { get; set; }
        public int? textareaRows { get; set; }
        public int? textareaCols { get; set; }
        public string controlGroupName { get; set; }
        public bool? controlChecked { get; set; }
        public string controlLabel { get; set; }
        public string controlValue { get; set; }
        public int? controlMin { get; set; }
        public int? controlMax { get; set; }
        public decimal? controlStep { get; set; }

        public List<AssessItem> loaderForSource { get; set; }

        public List<InpatientChartSource> sourceRadios { get; set; }


        public int hour { get; set; }
        public int minute { get; set; }
        public string date { get; set; }

        public string dateMeasured { get; set; }

        public int admissionId { get; set; }

        public Note mdtNote { get; set; }

        public bool? highlightSource { get; set; }
        public string highlightColor { get; set; }


        public string sourceTextValue { get; set; }
        public decimal sourceDecimalValue { get; set; }
        public int sourceIntValue { get; set; }
        public bool sourceBoolValue { get; set; }
        public DateTime sourceDateValue { get; set; }


        public string dailyChartSelectValue { get; set; }

        public Guid usr { get; set; }

    }
}
