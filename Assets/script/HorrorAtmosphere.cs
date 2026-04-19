using UnityEngine;

public class HorrorAtmosphere : MonoBehaviour
{
    [Header("Lighting")]
    public Light directionalLight;
    public float minLightIntensity = 0.1f;
    public float maxLightIntensity = 0.3f;
    public float flickerSpeed = 0.5f;
    
    [Header("Fog")]
    public bool enableFog = true;
    public Color fogColor = new Color(0.1f, 0.1f, 0.15f);
    public float fogDensity = 0.05f;
    
    [Header("Flashlight")]
    public Light flashlight;
    public float flashlightIntensity = 2f;
    public float flashlightRange = 15f;
    public bool flickeringFlashlight = false;
    public float flickerChance = 0.02f;
    
    [Header("Audio")]
    public AudioSource ambientAudio;
    public AudioClip[] randomScaryNoises;
    public float minTimeBetweenNoises = 10f;
    public float maxTimeBetweenNoises = 30f;
    private float nextNoiseTime;
    
    [Header("Vignette Effect (ขอบมืด)")]
    public bool enableVignette = true;
    public float vignetteIntensity = 0.4f;
    
    [Header("Heartbeat")]
    public AudioSource heartbeatAudio;
    public AudioClip heartbeatSound;
    public float heartbeatSpeed = 1f; // 1 = ปกติ, 2 = เร็ว
    private float heartbeatTimer = 0f;
    
    private HorrorCameraEffects cameraEffects;
    
    void Start()
    {
        cameraEffects = GetComponent<HorrorCameraEffects>();
        
        // ตั้งค่า Fog
        if (enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogMode = FogMode.Exponential;
        }
        
        // ตั้งค่า Flashlight
        if (flashlight)
        {
            flashlight.intensity = flashlightIntensity;
            flashlight.range = flashlightRange;
        }
        
        // ตั้งเวลาเสียงน่ากลัวครั้งแรก
        nextNoiseTime = Time.time + Random.Range(minTimeBetweenNoises, maxTimeBetweenNoises);
    }
    
    void Update()
    {
        // Light Flickering
        if (directionalLight)
        {
            float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
            directionalLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, flicker);
        }
        
        // Flashlight Flickering
        if (flashlight && flickeringFlashlight)
        {
            if (Random.value < flickerChance)
            {
                flashlight.enabled = !flashlight.enabled;
                Invoke("RestoreFlashlight", 0.1f);
            }
        }
        
        // เปิด/ปิด Flashlight ด้วย F
        if (flashlight && Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
        
        // Random Scary Noises
        if (Time.time >= nextNoiseTime && randomScaryNoises.Length > 0)
        {
            PlayRandomScaryNoise();
            nextNoiseTime = Time.time + Random.Range(minTimeBetweenNoises, maxTimeBetweenNoises);
        }
        
        // Heartbeat Effect
        UpdateHeartbeat();
    }
    
    void RestoreFlashlight()
    {
        if (flashlight)
            flashlight.enabled = true;
    }
    
    void PlayRandomScaryNoise()
    {
        if (ambientAudio && randomScaryNoises.Length > 0)
        {
            AudioClip clip = randomScaryNoises[Random.Range(0, randomScaryNoises.Length)];
            ambientAudio.PlayOneShot(clip);
            Debug.Log("Played scary noise!");
        }
    }
    
    void UpdateHeartbeat()
    {
        if (heartbeatAudio && heartbeatSound)
        {
            heartbeatTimer += Time.deltaTime * heartbeatSpeed;
            
            // เล่นเสียงหัวใจเต้นทุกๆ 1 วินาที (ปรับตาม Speed)
            if (heartbeatTimer >= 1f)
            {
                heartbeatAudio.PlayOneShot(heartbeatSound);
                heartbeatTimer = 0f;
            }
        }
    }
    
    // เรียกเมื่อเกิดเหตุการณ์น่ากลัว
    public void TriggerScareEvent(float fearAmount = 0.5f, float duration = 2f)
    {
        if (cameraEffects)
        {
            cameraEffects.ShakeCamera(0.2f, 0.5f);
            cameraEffects.SetFearLevel(fearAmount);
            
            // ค่อยๆ ลดความกลัว
            StartCoroutine(ReduceFearOverTime(duration));
        }
        
        // เพิ่มความเร็วหัวใจเต้น
        heartbeatSpeed = 2f;
        Invoke("NormalizeHeartbeat", duration);
    }
    
    System.Collections.IEnumerator ReduceFearOverTime(float duration)
    {
        float timer = 0f;
        float startFear = cameraEffects.fearLevel;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float fearLevel = Mathf.Lerp(startFear, 0f, timer / duration);
            cameraEffects.SetFearLevel(fearLevel);
            yield return null;
        }
        
        cameraEffects.SetFearLevel(0f);
    }
    
    void NormalizeHeartbeat()
    {
        heartbeatSpeed = 1f;
    }
    
    // เปิด/ปิด Flashlight ด้วย Code
    public void SetFlashlightState(bool state)
    {
        if (flashlight)
            flashlight.enabled = state;
    }
    
    // ทำให้ Flashlight กระพริบ
    public void FlickerFlashlight(float duration)
    {
        StartCoroutine(FlickerCoroutine(duration));
    }
    
    System.Collections.IEnumerator FlickerCoroutine(float duration)
    {
        float timer = 0f;
        bool originalState = flashlight.enabled;
        
        while (timer < duration)
        {
            flashlight.enabled = Random.value > 0.5f;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
            timer += Time.deltaTime;
        }
        
        flashlight.enabled = originalState;
    }
}