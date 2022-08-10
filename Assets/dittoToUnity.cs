// https://json2csharp.com/
using System.Collections.Generic;

namespace dittoClasses2 {
    public class Headers
    {
        public string MqttQos { get; set; }
        public string MqttRetain { get; set; }
        public string MqttTopic { get; set; }
        public string CorrelationId { get; set; }
        public string DittoOriginator { get; set; }
        public bool ResponseRequired { get; set; }
        public int version { get; set; }
        public List<object> RequestedAcks { get; set; }
        public string ContentType { get; set; }
    }
    public class Properties
    {
        public string Speed { get; set; }
        public string Position { get; set; }
        public string PositionMin { get; set; }
        public string PositionMax { get; set; }
    }
    public class Joint1
    {
        public Properties properties { get; set; }
    }
    public class Gripper
    {
        public Properties properties { get; set; }
    }
    public class Joint6
    {
        public Properties properties { get; set; }
    }
    public class Joint2
    {
        public Properties properties { get; set; }
    }
    public class Joint3
    {
        public Properties properties { get; set; }
    }
    public class Joint4
    {
        public Properties properties { get; set; }
    }
    public class Joint5
    {
        public Properties properties { get; set; }
    }
    public class Value
    {
        public Joint1 Joint1 { get; set; }
        public Gripper Gripper { get; set; }
        public Joint6 Joint6 { get; set; }
        public Joint2 Joint2 { get; set; }
        public Joint3 Joint3 { get; set; }
        public Joint4 Joint4 { get; set; }
        public Joint5 Joint5 { get; set; }
    }
    public class DittoToUnity
    {
        public string topic { get; set; }
        public Headers headers { get; set; }
        public string path { get; set; }
        public Value value { get; set; }
        public int revision { get; set; }
        public string timestamp { get; set; }
    }
}
