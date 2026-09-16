using UnityEngine;

public class MaterialTexOffsetCurve : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem particleSystem;

    [Header("Material Shape 1 X Offset")]
    public AnimationCurve offsetCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(1f, 1f)
    );

    [Header("Animation")]
    public float duration = 1f;

    private Renderer targetRenderer;
    private Material materialInstance;

    private float curveTimer;
    private float previousParticleTime;

    private bool isAnimating;
    private bool wasPlaying;
    private bool startDetected;

    private static readonly int MainTexST =
        Shader.PropertyToID("_MainTex_ST");

    private void Awake()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem == null)
        {
            Debug.LogError("MaterialTexOffsetCurve: Particle System not found.");
            enabled = false;
            return;
        }

        targetRenderer = particleSystem.GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogError("MaterialTexOffsetCurve: Renderer not found.");
            enabled = false;
            return;
        }

        // Create our own material instance.
        materialInstance = targetRenderer.material;

        // Initial state.
        SetXOffset(0f);

        previousParticleTime = particleSystem.time;
    }

    private void Update()
    {
        if (particleSystem == null || materialInstance == null)
            return;

        bool playing = particleSystem.isPlaying;
        float currentParticleTime = particleSystem.time;

        // =========================================================
        // PARTICLE SYSTEM HAS STARTED PLAYING
        // =========================================================

        if (playing && !wasPlaying)
        {
            StartOffsetAnimation();
        }

        // =========================================================
        // PARTICLE SYSTEM WAS RESTARTED WHILE ALREADY PLAYING
        //
        // Unity can keep isPlaying == true when Play() is called
        // again. In that case, detect the particle time jumping
        // backwards.
        // =========================================================

        if (playing &&
            currentParticleTime < previousParticleTime - 0.01f)
        {
            StartOffsetAnimation();
        }

        // =========================================================
        // ARM THE RESTART DETECTION AFTER PARTICLE TIME MOVES
        // FORWARD.
        //
        // This prevents the curve from being restarted every
        // frame while particle time is near zero.
        // =========================================================

        if (playing && currentParticleTime > 0.05f)
        {
            startDetected = false;
        }

        wasPlaying = playing;
        previousParticleTime = currentParticleTime;

        // =========================================================
        // CURVE ANIMATION
        // =========================================================

        if (!isAnimating)
            return;

        if (duration <= 0f)
        {
            SetXOffset(offsetCurve.Evaluate(1f));
            isAnimating = false;
            return;
        }

        curveTimer += Time.deltaTime;

        float normalizedTime =
            Mathf.Clamp01(curveTimer / duration);

        float offset =
            offsetCurve.Evaluate(normalizedTime);

        SetXOffset(offset);

        if (normalizedTime >= 1f)
        {
            isAnimating = false;
        }
    }

    private void StartOffsetAnimation()
    {
        // Prevent duplicate triggers.
        if (startDetected)
            return;

        startDetected = true;

        // ---------------------------------------------------------
        // RESET FIRST
        // ---------------------------------------------------------

        SetXOffset(0f);

        // ---------------------------------------------------------
        // RESTART CURVE FROM THE BEGINNING
        // ---------------------------------------------------------

        curveTimer = 0f;
        isAnimating = true;
    }

    private void SetXOffset(float value)
    {
        Vector4 mainTexST =
            materialInstance.GetVector(MainTexST);

        // _MainTex_ST
        //
        // X = Tiling X
        // Y = Tiling Y
        // Z = Offset X
        // W = Offset Y

        mainTexST.z = value;

        materialInstance.SetVector(
            MainTexST,
            mainTexST
        );
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}