
//https://gist.github.com/scottgarner/7fb960ff67c3f6091f9dbc741179d634
//https://socketsbay.com/test-websockets
// [SerializeField] public string webSocketURL = "wss://socketsbay.com/wss/v2/2/demo/";
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using System.Collections;
// using System.IO;
// using System.Linq;
// using dittoClasses1;
// using dittoClasses2;
using UnityEngine;
using UnityEngine.UI;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text;
using Newtonsoft.Json;
public class RobotControl : MonoBehaviour {
    public GameObject localControl;
    public bool localController;
    [SerializeField] public string webSocketURL = "ws://@localhost:8082/ws/2/";
    public delegate void ReceiveAction(string message);
    public event ReceiveAction OnReceived;
    private ClientWebSocket webSocket = null;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

/* #region sliders */
    public Slider JT1Slider;
    public Slider JT2Slider;
    public Slider JT3Slider;
    public Slider JT4Slider;
    public Slider JT5Slider;
    public Slider JT6Slider;
    public Slider GripperSlider;
    //Create slider variable with default value
    public float JT1Value = 0.0f;
    public float JT2Value = 0.0f;
    public float JT3Value = 0.0f;
    public float JT4Value = 0.0f;
    public float JT5Value = 0.0f;
    public float JT6Value = 0.0f;
    public float GripperValue = 0.0f;
    //Where the appropriate arm parts are plugged into the inspector
    public Transform JT1;
    public Transform JT2;
    public Transform JT3;
    public Transform JT4;
    public Transform JT5;
    public Transform JT6;
    public Transform GripperA;
    public Transform GripperB;
    //Rotation speed
    public float JT1Rate = 1.0f;
    //Where rotation starts (starts at 0)
    private float JT1Rotation = 0.0f;
    //Rotation degrees to the negative side
    public float JT1RotationMin = -135.0f;
    //Rotation degrees to the positive side (clockwise)
    public float JT1RotationMax = 135.0f;
    public float JT2Rate = 1.0f;
    private float JT2Rotation = 0.0f;
    public float JT2RotationMin = -90.0f;
    public float JT2RotationMax = 90.0f;
    public float JT3Rate = 1.0f;
    private float JT3Rotation = 0.0f;
    public float JT3RotationMin = -77.5f;
    public float JT3RotationMax = 77.5f;
    public float JT4Rate = 1.0f;
    private float JT4Rotation = 0.0f;
    public float JT4RotationMin = -90.0f;
    public float JT4RotationMax = 90.0f;
    public float JT5Rate = 1.0f;
    private float JT5Rotation = 0.0f;
    public float JT5RotationMin = -90.0f;
    public float JT5RotationMax = 90.0f;
    public float JT6Rate = 1.0f;
    private float JT6Rotation = 0.0f;
    public float JT6RotationMin = -135.0f;
    public float JT6RotationMax = 135.0f;
    public float GripperRate = 0.005f;
    private float GripperTranslation = 0.0f;
    public float GripperTranslationMin = -0.0100f;
    public float GripperTranslationMax = 0.0200f;

    /* #endregion */
    
    void Start() {
        localController = localControl.GetComponent<Toggle>().isOn;
        Task connect = Connect(webSocketURL);
        //Set default values to make possible to the UI sliders that values from -1 to 1
        JT1Slider.minValue = -1;
        JT1Slider.maxValue = 1;
        JT2Slider.minValue = -1;
        JT2Slider.maxValue = 1;
        JT3Slider.minValue = -1;
        JT3Slider.maxValue = 1;
        JT4Slider.minValue = -1;
        JT4Slider.maxValue = 1;
        JT5Slider.minValue = -1;
        JT5Slider.maxValue = 1;
        JT6Slider.minValue = -1;
        JT6Slider.maxValue = 1;
        GripperSlider.minValue = -1;
        GripperSlider.maxValue = 1;
    }
    void OnDestroy() {
        if (cancellationTokenSource != null) {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
            if(webSocket != null)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    Task.Run(()=> webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, "", cancellationToken));
                }
            }
        }
        Debug.Log("<color=cyan>WebSocket closed</color>");
    }
    public async Task Connect(string url) {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        while (!cancellationToken.IsCancellationRequested) {
            using (webSocket = new ClientWebSocket()) {
                Debug.Log("<color=cyan>Websocket</color>");
                try {
                    Debug.Log($"<color=cyan>Establishing WebSocket communication with: {url}</color>");
                    string username = "ditto";
                    string password = "ditto";
                    string auth = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                    webSocket.Options.SetRequestHeader("Authorization", "Basic " + auth);
                    await webSocket.ConnectAsync(new Uri(url), cancellationToken);
                    await Task.Run(()=> Send("START-SEND-EVENTS"));
                    Debug.Log($"<color=cyan>Connected to: {webSocketURL}</color>");
                    await Receive();
                    Debug.Log("<color=cyan>WebSocket closed</color>");
                }
                catch (OperationCanceledException) {
                    Debug.Log("<color=cyan>WebSocket shutting down</color>");
                }
                catch (WebSocketException) {
                    Debug.Log("<color=cyan>WebSocket connection lost</color>");
                }
                catch (Exception ex) {
                    Debug.Log(ex);
                    throw;
                }
            }
            await Task.Delay(1000);
        }
    }
    public void Send(string register) {
        if (webSocket != null && webSocket.State == WebSocketState.Open) {
            var encoded = Encoding.UTF8.GetBytes(register);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }
    }
    public void SendMovement() {
        if(localController)
        {
            string register;
            var info = new dittoClasses1.UnityToDitto();
            info.topic = "IoT/Omron/things/twin/events/modified";
            info.headers.MqttQos = "0";
            info.headers.MqttRetain = "false";
            info.headers.MqttTopic = "DTw/IoT:Omron";
            info.headers.CorrelationId = "b6ba1470-f84a-424f-b132-8659878d95da";
            info.headers.DittoOriginator = "nginx:ditto";
            info.headers.ResponseRequired = false;
            info.headers.version = 2;
            info.headers.ContentType = "application/json";
            info.path = "/features";
            info.thingId = "IoT:Omron";
            info.policyId = "DTw:policy";
            info.definition = "Robot:joints:1.0.0";
            info.attributes.manufacturer = "Omron";
            info.attributes.serial = "TM5A-700";
            info.features.Gripper.properties.Speed = (GripperRate).ToString();
            info.features.Gripper.properties.Position = (GripperTranslation + GripperValue * GripperRate).ToString();
            info.features.Gripper.properties.PositionMin = (GripperTranslationMin).ToString();
            info.features.Gripper.properties.PositionMax = (GripperTranslationMax).ToString();
            info.features.Joint1.properties.Speed = (JT1Rate).ToString();
            info.features.Joint1.properties.Position = (JT1Rotation + JT1Value * JT1Rate).ToString();
            info.features.Joint1.properties.PositionMin = (JT1RotationMin).ToString();
            info.features.Joint1.properties.PositionMax = (JT1RotationMax).ToString();
            info.features.Joint2.properties.Speed = (JT2Rate).ToString();
            info.features.Joint2.properties.Position = (JT2Rotation + JT2Value * JT2Rate).ToString();
            info.features.Joint2.properties.PositionMin = (JT2RotationMin).ToString();
            info.features.Joint2.properties.PositionMax = (JT2RotationMax).ToString();
            info.features.Joint3.properties.Speed = (JT3Rate).ToString();
            info.features.Joint3.properties.Position = (JT3Rotation + JT3Value * JT3Rate).ToString();
            info.features.Joint3.properties.PositionMin = (JT3RotationMin).ToString();
            info.features.Joint3.properties.PositionMax = (JT3RotationMax).ToString();
            info.features.Joint4.properties.Speed = (JT4Rate).ToString();
            info.features.Joint4.properties.Position = (JT4Rotation + JT4Value * JT4Rate).ToString();
            info.features.Joint4.properties.PositionMin = (JT4RotationMin).ToString();
            info.features.Joint4.properties.PositionMax = (JT4RotationMax).ToString();
            info.features.Joint5.properties.Speed = (JT5Rate).ToString();
            info.features.Joint5.properties.Position = (JT5Rotation + JT5Value * JT5Rate).ToString();
            info.features.Joint5.properties.PositionMin = (JT5RotationMin).ToString();
            info.features.Joint5.properties.PositionMax = (JT5RotationMax).ToString();
            info.features.Joint6.properties.Speed = (JT6Rate).ToString();
            info.features.Joint6.properties.Position = (JT6Rotation + JT6Value * JT6Rate).ToString();
            info.features.Joint6.properties.PositionMin = (JT6RotationMin).ToString();
            info.features.Joint6.properties.PositionMax = (JT6RotationMax).ToString();
            register = JsonConvert.SerializeObject(info);
            Send(register);
            Debug.Log($"Registo enviado: {register}");
            JT1Rotation += JT1Value * JT1Rate;
            JT2Rotation += JT2Value * JT2Rate;
            JT3Rotation += JT3Value * JT3Rate;
            JT4Rotation += JT4Value * JT4Rate;
            JT5Rotation += JT5Value * JT5Rate;
            JT6Rotation += JT6Value * JT6Rate;
            GripperTranslation += GripperValue * GripperRate;
            ProcessMovement();
        }
    }
    public async Task Receive() {
        var arraySegment = new ArraySegment<byte>(new byte[8192]);
        while (webSocket.State == WebSocketState.Open) {
            var result = await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text) {
                var message = Encoding.UTF8.GetString(arraySegment.Array, 0, result.Count);
                Debug.Log($"Received data: {message}");
                try
                {
                    if (!localController)
                    {
                        var obj = JsonConvert.DeserializeObject<dittoClasses2.DittoToUnity>(message);
                        JT1Rotation = float.Parse(obj.value.Joint1.properties.Position);
                        JT2Rotation = float.Parse(obj.value.Joint2.properties.Position);
                        JT3Rotation = float.Parse(obj.value.Joint3.properties.Position);
                        JT4Rotation = float.Parse(obj.value.Joint4.properties.Position);
                        JT5Rotation = float.Parse(obj.value.Joint5.properties.Position);
                        JT6Rotation = float.Parse(obj.value.Joint6.properties.Position);
                        GripperTranslation = float.Parse(obj.value.Gripper.properties.Position);
                        ProcessMovement();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log($"Exception: {e.Message}");
                }
                // https://stackoverflow.com/questions/3142495/deserialize-json-into-c-sharp-dynamic-object
                // var data = JsonUtility.ToJson(message);
                // var parsedData = JsonUtility.FromJson<Root>(data);
                // // var props = string.Join(" ",
                // //         parsedData.GetType()
                // //         .GetProperties()
                // //         .Select(prop => prop.GetValue(parsedData))
                // // );
                // // Debug.Log($"props:\n{props}");
                // Debug.Log($"Data: {parsedData.value.Teste.properties.Dia}");
                if (OnReceived != null) OnReceived("message");
            }
        }
    }
    void CheckInput() {
        //Check the value of the UI slider
        JT1Value = JT1Slider.value;
        JT2Value = JT2Slider.value;
        JT3Value = JT3Slider.value;
        JT4Value = JT4Slider.value;
        JT5Value = JT5Slider.value;
        JT6Value = JT6Slider.value;
        GripperValue = GripperSlider.value;
    }
    void ProcessMovement() {
        //The rotation value multiplying the slider and the turn rate values (JT1Rotation is the angle value)
        // JT1Rotation += JT1Value * JT1Rate;
        JT1Rotation = Mathf.Clamp(JT1Rotation, JT1RotationMin, JT1RotationMax);
        //Rotate the joint around the Y axis
        JT1.localEulerAngles = new Vector3(JT1.localEulerAngles.x, JT1Rotation, JT1.localEulerAngles.z);
        // JT2Rotation += JT2Value * JT2Rate;
        JT2Rotation = Mathf.Clamp(JT2Rotation, JT2RotationMin, JT2RotationMax);
        JT2.localEulerAngles = new Vector3(JT2.localEulerAngles.x, JT2Rotation, JT2.localEulerAngles.z);
        // JT3Rotation += JT3Value * JT3Rate;
        JT3Rotation = Mathf.Clamp(JT3Rotation, JT3RotationMin, JT3RotationMax);
        JT3.localEulerAngles = new Vector3(JT3.localEulerAngles.x, JT3Rotation, JT3.localEulerAngles.z);
        // JT4Rotation += JT4Value * JT4Rate;
        JT4Rotation = Mathf.Clamp(JT4Rotation, JT4RotationMin, JT4RotationMax);
        JT4.localEulerAngles = new Vector3(JT4.localEulerAngles.x, JT4Rotation, JT4.localEulerAngles.z);
        // JT5Rotation += JT5Value * JT5Rate;
        JT5Rotation = Mathf.Clamp(JT5Rotation, JT5RotationMin, JT5RotationMax);
        JT5.localEulerAngles = new Vector3(JT5.localEulerAngles.x, JT5Rotation, JT5.localEulerAngles.z);
        // JT6Rotation += JT6Value * JT6Rate;
        JT6Rotation = Mathf.Clamp(JT6Rotation, JT6RotationMin, JT6RotationMax);
        JT6.localEulerAngles = new Vector3(JT6.localEulerAngles.x, JT6Rotation, JT6.localEulerAngles.z);
        // GripperTranslation += GripperValue * GripperRate;
        GripperTranslation = Mathf.Clamp(GripperTranslation, GripperTranslationMin, GripperTranslationMax);
        GripperA.localPosition = new Vector3(GripperTranslation, 0.0f, 0.0f);
        GripperB.localPosition = new Vector3(GripperTranslation, 0.0f, 0.0f);
    }
    public void ResetSliders() {
        //Reset sliders back to 0 when lift up the mouse click down (snapping effect)
        JT1Value = 0.0f;
        JT1Slider.value = 0.0f;
        JT2Value = 0.0f;
        JT2Slider.value = 0.0f;
        JT3Value = 0.0f;
        JT3Slider.value = 0.0f;
        JT4Value = 0.0f;
        JT4Slider.value = 0.0f;
        JT5Value = 0.0f;
        JT5Slider.value = 0.0f;
        JT6Value = 0.0f;
        JT6Slider.value = 0.0f;
        GripperValue = 0.0f;
        GripperSlider.value = 0.0f;
    }
    public void Update() {
        localController = localControl.GetComponent<Toggle>().isOn;
        CheckInput();
        ProcessMovement();
        SendMovement();
    }
}