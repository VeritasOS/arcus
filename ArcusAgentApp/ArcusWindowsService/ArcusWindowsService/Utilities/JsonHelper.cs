/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using Newtonsoft.Json;

namespace BEArcus.Agent
{
    class JsonHelper
    {
        ///<Summary>Dererializes the Json string.</Summary>  
        public static T JsonDeserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        ///<Summary>Serialize to Json String.</Summary>  
        public static string JsonSerializer<T>(T dataObject)
        {
            return JsonConvert.SerializeObject(dataObject, Formatting.Indented);
        }
    }
}
