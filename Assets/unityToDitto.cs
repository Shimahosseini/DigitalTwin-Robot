// https://json2csharp.com/
using System.Collections.Generic;
using System;

namespace dittoClasses1 {
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
    public class Attributes
    {
        public string manufacturer { get; set; }
        public string serial { get; set; }
    }
    public class Properties
    {
        public string Speed { get; set; }
        public string Position { get; set; }
        public string PositionMin { get; set; }
        public string PositionMax { get; set; }
    }
    public class Gripper
    {
        public Properties properties { get; set; }
        public Gripper()
        {
            properties = new Properties();
        }
    }
    public class Joint6
    {
        public Properties properties { get; set; }
        public Joint6()
        {
            properties = new Properties();
        }
    }
    public class Joint5
    {
        public Properties properties { get; set; }
        public Joint5()
        {
            properties = new Properties();
        }
    }
    public class Joint4
    {
        public Properties properties { get; set; }
        public Joint4()
        {
            properties = new Properties();
        }
    }
    public class Joint3
    {
        public Properties properties { get; set; }
        public Joint3()
        {
            properties = new Properties();
        }
    }
    public class Joint2
    {
        public Properties properties { get; set; }
        public Joint2()
        {
            properties = new Properties();
        }
    }
    public class Joint1
    {
        public Properties properties { get; set; }
        public Joint1()
        {
            properties = new Properties();
        }
    }
    public class Features
    {
        public Gripper Gripper { get; set; }
        public Joint6 Joint6 { get; set; }
        public Joint5 Joint5 { get; set; }
        public Joint4 Joint4 { get; set; }
        public Joint3 Joint3 { get; set; }
        public Joint2 Joint2 { get; set; }
        public Joint1 Joint1 { get; set; }
        public Features()
        {
            Gripper = new Gripper();
            Joint6 = new Joint6();
            Joint5 = new Joint5();
            Joint4 = new Joint4();
            Joint3 = new Joint3();
            Joint2 = new Joint2();
            Joint1 = new Joint1();
        }
    }
    public class UnityToDitto
    {        
        public string topic { get; set; }
        public Headers headers { get; set; }
        public string path { get; set; }
        public string thingId { get; set; }
        public string policyId { get; set; }
        public string definition { get; set; }
        public Attributes attributes { get; set; }
        public Features features { get; set; }
        public UnityToDitto()
        {
            headers = new Headers();
            attributes = new Attributes();
            features = new Features();
        }
    }
}