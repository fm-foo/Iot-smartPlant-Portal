using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Models {
    public class InfluxQuery {
        public DateTime TimeStamp { get; set; }
        public string Field { get; set; }
        public object Value { get; set; }



        public InfluxQuery(object timeStamp, string field, object value) {
            TimeStamp = Convert.ToDateTime(timeStamp.ToString());
            Field = field;
            Value = value;
        }
    }
}
