using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, ISubscribeUnsubscribe
{
    Camera cam;
    Transform anchor;
    GameManager gm;

    public int SubscribedTimes { get; set; }

    [SerializeField] Transform target;

    public CameraSettings cameraInGameSettings;

    [Space(25)]
    public Vector3 OriginalStartAngle;
    public Vector3 OriginalCharOffSet;

    [Space(25)]
    public float chaseSpeed;/*
    [SerializeField] Vector3 chaseOffset; // default values were: 0, 7.5, -8
    [SerializeField] Vector3 defaultPosition;*/

    public enum CameraState
    {
        None,
        Chase,
        FinishLine
    }

    CameraState state;
    public CameraState State
    {
        get => state;
        set
        {
            if (value != state)
            {
                switch (value)
                {
                    case CameraState.Chase:

                        if (anchor)
                            Destroy(anchor.gameObject);

                        anchor = new GameObject("cameraAnchor").transform;
                        anchor.SetParent(target);
                        anchor.localPosition += cameraInGameSettings.chaseOffset;
                        transform.eulerAngles = cameraInGameSettings.startingAngle;
                        cameraInGameSettings.skippedCutscene = false;
                        chaseSpeed = cameraInGameSettings.chaseSpeed;

                        break;

                    case CameraState.FinishLine:
                        if(gm == null) gm = GameManager.GetInstance();

                        if(gm.IsPlayerFirstToFinish())
                        {
                            transform.eulerAngles = cameraInGameSettings.finishLineAngle_WINNING;
                            anchor.localPosition = cameraInGameSettings.finishLinechaseOffset_WINNING;
						}
                        else
                        {
                            transform.eulerAngles = cameraInGameSettings.finishLineAngle_LOSING;
                            anchor.localPosition = cameraInGameSettings.finishLinechaseOffset_LOSING;
                        }
                        break;
                }
            }

            state = value;
        }
    }

    [System.Serializable]
    public struct CameraSettings
    {
        public Vector3 startingAngle;
        public Vector3 chaseOffset;

        public float chaseSpeed;

        public Vector3 finishLineAngle_WINNING;
        public Vector3 finishLinechaseOffset_WINNING;

        public Vector3 finishLineAngle_LOSING;
        public Vector3 finishLinechaseOffset_LOSING;

        public Vector3 defaultPosition;

        public bool skippedCutscene;
    }

    void Start()
    {
        Subscribe();

        cam = GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    void LateUpdate()
    {
        if ( state == CameraState.Chase || state == CameraState.FinishLine )
        {
            if (anchor /*&& !cameraInGameSettings.skippedCutscene*/ )
                cam.transform.position = Vector3.Lerp(cam.transform.position, anchor.position, chaseSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;

        State = CameraState.Chase;
    }

    void OnChangeGameState(GameState _newState)
    {
        if(_newState == GameState.LoadLevel)
        {
            State = CameraState.None;
            transform.position = cameraInGameSettings.defaultPosition;
            transform.eulerAngles = cameraInGameSettings.startingAngle;
        }
        if(_newState == GameState.FinishLine)
        {
            State = CameraState.FinishLine;
        }
    }

    void SkippedCutScene()
    {
        cameraInGameSettings.skippedCutscene = true;

        chaseSpeed = 1000f;
    }

	public void Subscribe()
	{
        ++SubscribedTimes;

        //print($"CameraController: {SubscribedTimes}");

        CustomGameEventList.OnChangeGameState += OnChangeGameState;
        CustomGameEventList.CutSceneSkipped += SkippedCutScene;
        //CustomGameEventList.SkipLevel += SkippedCutScene;
    }

	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        CustomGameEventList.OnChangeGameState -= OnChangeGameState;
        CustomGameEventList.CutSceneSkipped -= SkippedCutScene;
        //CustomGameEventList.SkipLevel -= SkippedCutScene;
    }
}
