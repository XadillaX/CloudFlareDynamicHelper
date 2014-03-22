using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFlareDynamicHelper
{
    class ValidRecord
    {
        public ValidRecord(String REC_ID, String DISPLAY_NAME, String ZONE_NAME, String TYPE, String CONTENT)
        {
            rec_id = REC_ID;
            display_name = DISPLAY_NAME;
            zone_name = ZONE_NAME;
            type = TYPE;
            content = CONTENT;
        }

        public String display_name;
        public String zone_name;
        public String content;

        public String rec_id;
        public String type;
    }
}
