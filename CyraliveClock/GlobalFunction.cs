using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace CyraliveClock
{
    internal class GlobalFunction
    {
        public static JsonNode getCyraliveConfig;
        public static bool Cierra_hold_position;
        public static string[] get_position;
        public static bool stylechange = false;
        public static void write_config_file(string item, dynamic value)
        {
            JsonNode modify_current_json = JsonNode.Parse(File.ReadAllText("CyraliveClock.json"));
            modify_current_json[item] = value;
            File.WriteAllText("CyraliveClock.json", modify_current_json.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }

        public static string read_config_file(string item)
        {
            JsonDocument getCyraliveConfig = JsonDocument.Parse(File.ReadAllText("CyraliveClock.json"));
            return getCyraliveConfig.RootElement.GetProperty(item).ToString();
        }
    }
}
