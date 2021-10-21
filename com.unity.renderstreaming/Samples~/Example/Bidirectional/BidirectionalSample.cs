using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.RenderStreaming.Samples
{
    class BidirectionalSample : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private RenderStreaming renderStreaming;
        [SerializeField] private Dropdown webcamSelectDropdown;
        [SerializeField] private Dropdown microphoneSelectDropdown;
        [SerializeField] private Button startVideoButton;
        [SerializeField] private Button startMicButton;
        [SerializeField] private Button setUpButton;
        [SerializeField] private Button hangUpButton;
        [SerializeField] private InputField connectionIdInput;
        [SerializeField] private RawImage localVideoImage;
        [SerializeField] private RawImage remoteVideoImage;
        [SerializeField] private AudioSource receiveAudioSource;
        [SerializeField] private WebCamStreamer webCamStreamer;
        [SerializeField] private ReceiveVideoViewer receiveVideoViewer;
        [SerializeField] private MicrophoneStreamer microphoneStreamer;
        [SerializeField] private ReceiveAudioViewer receiveAudioViewer;
        [SerializeField] private SingleConnection singleConnection;
#pragma warning restore 0649

        private string connectionId;

        void Awake()
        {
            startVideoButton.interactable = true;
            webcamSelectDropdown.interactable = true;
            setUpButton.interactable = false;
            hangUpButton.interactable = false;
            connectionIdInput.interactable = true;
            startVideoButton.onClick.AddListener(() =>
            {
                webCamStreamer.enabled = true;
                startVideoButton.interactable = false;
                webcamSelectDropdown.interactable = false;
                setUpButton.interactable = true;
            });
            startMicButton.onClick.AddListener(() =>
            {
                microphoneStreamer.enabled = true;
                startMicButton.interactable = false;
                microphoneSelectDropdown.interactable = false;
                setUpButton.interactable = true;
            });
            setUpButton.onClick.AddListener(SetUp);
            hangUpButton.onClick.AddListener(HangUp);
            connectionIdInput.onValueChanged.AddListener(input => connectionId = input);
            connectionIdInput.text = $"{Random.Range(0, 99999):D5}";
            webcamSelectDropdown.onValueChanged.AddListener(index => webCamStreamer.SetDeviceIndex(index));
            webcamSelectDropdown.options =
                webCamStreamer.WebCamNameList.Select(x => new Dropdown.OptionData(x)).ToList();
            webCamStreamer.OnStartedStream += id => receiveVideoViewer.enabled = true;
            webCamStreamer.OnUpdateWebCamTexture += texture => localVideoImage.texture = texture;
            receiveVideoViewer.OnUpdateReceiveTexture += texture => remoteVideoImage.texture = texture;
            microphoneSelectDropdown.onValueChanged.AddListener(index => microphoneStreamer.SetDeviceIndex(index));
            microphoneSelectDropdown.options =
                microphoneStreamer.MicrophoneNameList.Select(x => new Dropdown.OptionData(x)).ToList();
            microphoneStreamer.OnStartedStream += id => receiveAudioViewer.enabled = true;
            receiveAudioViewer.OnUpdateReceiveAudioClip += clip =>
            {
                receiveAudioSource.clip = clip;
                receiveAudioSource.loop = true;
                receiveAudioSource.Play();
            };
        }

        void Start()
        {
            if (renderStreaming.runOnAwake)
                return;
            renderStreaming.Run(
                hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
                signaling: RenderStreamingSettings.Signaling);
        }

        private void SetUp()
        {
            setUpButton.interactable = false;
            hangUpButton.interactable = true;
            connectionIdInput.interactable = false;

            singleConnection.CreateConnection(connectionId);
        }

        private void HangUp()
        {
            singleConnection.DeleteConnection(connectionId);

            remoteVideoImage.texture = null;
            setUpButton.interactable = true;
            hangUpButton.interactable = false;
            connectionIdInput.interactable = true;
            connectionIdInput.text = $"{Random.Range(0, 99999):D5}";
        }
    }
}
