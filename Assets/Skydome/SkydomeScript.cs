using UnityEngine;
using System.Collections;

public class SkydomeScript : MonoBehaviour {
    
    
    private GameObject sunLight;
    public GameObject player;

    public float JULIANDATE = 150;
    public float LONGITUDE = 0.0f;
    public float LATITUDE = 0.0f;
    float MERIDIAN = 0.0f;
    public float TIME = 8.0f;
    public float TimeSpeedFactor = 0.01f;
    public float Turbidity = 2.0f;

    public float cloudSpeed1 = 1.0f;
    public float cloudSpeed2 = 1.5f;
    public float cloudHeight1 = 2.0f;
    public float cloudHeight2 = 4.0f;
    float cloudTint = 1.0f;

    Vector4 vBetaRayleigh = new Vector4();
    Vector4 vBetaMie = new Vector4();
    Vector3 m_vBetaRayTheta = new Vector3();
    Vector3 m_vBetaMieTheta = new Vector3();
    
    public float m_fRayFactor = 1000.0f;
	public float m_fMieFactor =  0.7f;
    float m_fDirectionalityFactor = 0.6f;
    public float m_fSunColorIntensity = 1.0f;
    
    //Sun Variables -----------
    Vector3 m_vDirection;
    Vector3 m_vColor;
    Vector3 sunDirection = new Vector3();
    Vector3 sunDirection2 = new Vector3();
    float SolarAzimuth;
    float solarAltitude;
    Vector3 sunPosition;
    float domeRadius = 10;
    float m_fTheta;
    float m_fPhi;
    float LATITUDE_RADIANS;
    float STD_MERIDIAN;
    //-------------------------


	void Start () 
	{
	    sunLight = new GameObject("Sun");
	    sunLight.AddComponent<Light>();
        sunLight.GetComponent<Light>().type = LightType.Directional;
	}
    
    void Update()
    {
        TIME += TimeSpeedFactor;
        if(TIME >=24) TIME = 0;
        this.transform.position = new Vector3(player.transform.position.x,player.transform.position.y - 50,player.transform.position.z);
        calcAtmosphere();

        Vector3 sunLightD = sunLight.transform.TransformDirection(-Vector3.forward);
        Vector3 pos = new Vector3(0f,0.02f,0f);

        this.GetComponent<Renderer>().material.SetVector("vBetaRayleigh", vBetaRayleigh);
        this.GetComponent<Renderer>().material.SetVector("BetaRayTheta", m_vBetaRayTheta);
        this.GetComponent<Renderer>().material.SetVector("vBetaMie", vBetaMie);                     
        this.GetComponent<Renderer>().material.SetVector("BetaMieTheta", m_vBetaMieTheta);
        this.GetComponent<Renderer>().material.SetVector("g_vEyePt",  pos);
        this.GetComponent<Renderer>().material.SetVector("LightDir", sunLightD);
        this.GetComponent<Renderer>().material.SetVector("g_vSunColor", m_vColor);
        this.GetComponent<Renderer>().material.SetFloat("DirectionalityFactor", m_fDirectionalityFactor);
        this.GetComponent<Renderer>().material.SetFloat("SunColorIntensity", m_fSunColorIntensity);
        this.GetComponent<Renderer>().material.SetFloat("tint", cloudTint);
        this.GetComponent<Renderer>().material.SetFloat("cloudSpeed1", cloudSpeed1);
        this.GetComponent<Renderer>().material.SetFloat("cloudSpeed2", cloudSpeed2);
        this.GetComponent<Renderer>().material.SetFloat("plane_height1", cloudHeight1);
        this.GetComponent<Renderer>().material.SetFloat("plane_height2", cloudHeight2);
        
        //update sun
        LATITUDE = Mathf.Clamp(LATITUDE, -90.0f, 90.0f);
        SetPosition2(TIME);
        sunLight.transform.position = sunPosition;

	}
    void calcAtmosphere()
    {
        calcRay();
        CalculateMieCoeff();
    }
    void calcRay()
    {
	    const float n  = 1.00029f;		//Refraction index for air
	    const float N  = 2.545e25f;		//Molecules per unit volume
	    const float pn = 0.035f;		//Depolarization factor

        float fRayleighFactor = m_fRayFactor * (Mathf.Pow(Mathf.PI, 2.0f) * Mathf.Pow(n * n - 1.0f, 2.0f) * (6 + 3 * pn)) / ( N * ( 6 - 7 * pn ) );
        
	    m_vBetaRayTheta.x = fRayleighFactor / ( 2.0f * Mathf.Pow( 650.0e-9f, 4.0f ) );
	    m_vBetaRayTheta.y = fRayleighFactor / ( 2.0f * Mathf.Pow( 570.0e-9f, 4.0f ) );
	    m_vBetaRayTheta.z = fRayleighFactor / ( 2.0f * Mathf.Pow( 475.0e-9f, 4.0f ) );

        vBetaRayleigh.x = 8.0f * fRayleighFactor / (3.0f * Mathf.Pow(650.0e-9f, 4.0f));
        vBetaRayleigh.y = 8.0f * fRayleighFactor / (3.0f * Mathf.Pow(570.0e-9f, 4.0f));
        vBetaRayleigh.z = 8.0f * fRayleighFactor / (3.0f * Mathf.Pow(475.0e-9f, 4.0f));
    }
    void CalculateMieCoeff()
    {
        float[] K =new float[3];
        K[0]=0.685f;  
        K[1]=0.679f;
        K[2]=0.670f;

	    float c = ( 0.6544f * Turbidity - 0.6510f ) * 1e-16f;	//Concentration factor

	    float fMieFactor = m_fMieFactor * 0.434f * c * 4.0f * Mathf.PI * Mathf.PI;

	    m_vBetaMieTheta.x = fMieFactor / ( 2.0f * Mathf.Pow( 650e-9f, 2.0f ) );
	    m_vBetaMieTheta.y = fMieFactor / ( 2.0f * Mathf.Pow( 570e-9f, 2.0f ) );
	    m_vBetaMieTheta.z = fMieFactor / ( 2.0f * Mathf.Pow( 475e-9f, 2.0f ) );

        vBetaMie.x = K[0] * fMieFactor / Mathf.Pow(650e-9f, 2.0f);
        vBetaMie.y = K[1] * fMieFactor / Mathf.Pow(570e-9f, 2.0f);
        vBetaMie.z = K[2] * fMieFactor / Mathf.Pow(475e-9f, 2.0f);

        float fTemp3 = 0.434f * c * (float)Mathf.PI * (2.0f * (float)Mathf.PI) * (2.0f * (float)Mathf.PI);
        // not sure if above is correct, but it look good.

        vBetaMie *= fTemp3;
    }
    
    
    //Sun Position Calcs -------------------------------------------
    void SetPosition(float fTheta, float fPhi )
    {
	    m_fTheta = fTheta;
	    m_fPhi = fPhi;

	    float fCosTheta = Mathf.Cos( m_fTheta );
        float fSinTheta = Mathf.Sin(m_fTheta);
        float fCosPhi = Mathf.Cos(m_fPhi);
        float fSinPhi = Mathf.Sin(m_fPhi);

	    m_vDirection = new Vector3( fSinTheta * fCosPhi,fCosTheta,fSinTheta * fSinPhi );
        float phiSun = (Mathf.PI * 2.0f) - SolarAzimuth;

        sunDirection.x = domeRadius;
        sunDirection.y = phiSun;
        sunDirection.z = solarAltitude;
        sunPosition = sphericalToCartesian(sunDirection);
        sunDirection2 = calcDirection(m_fTheta, phiSun);
        m_vDirection = Vector3.Normalize(m_vDirection);
        sunLight.transform.LookAt(sunDirection2);
        ComputeAttenuation();
    }
    
    void SetPosition2(float fTime)
    {
        //float JULIANDATE = skydomeScript.JULIANDATE;
        float sMERIDIAN = MERIDIAN * 15;
        float sLATITUDE = Mathf.Deg2Rad * LATITUDE;
        float sLONGITUDE =  LONGITUDE;

        float t = fTime + 0.170f * Mathf.Sin((4.0f * Mathf.PI * (JULIANDATE - 80.0f)) / 373.0f)
                     - 0.129f * Mathf.Sin((2.0f * Mathf.PI * (JULIANDATE - 8.0f)) / 355.0f)
                     + (12 * (sMERIDIAN - sLONGITUDE)) / Mathf.PI;

        float fDelta = 0.4093f * Mathf.Sin((2.0f * Mathf.PI * (JULIANDATE - 81.0f)) / 368.0f);

        float fSinLat = Mathf.Sin(sLATITUDE);
        float fCosLat = Mathf.Cos(sLATITUDE);
        float fSinDelta = Mathf.Sin(fDelta);
        float fCosDelta = Mathf.Cos(fDelta);
        float fSinT = Mathf.Sin((Mathf.PI * t) / 12.0f);
        float fCosT = Mathf.Cos((Mathf.PI * t) / 12.0f);

        float fTheta = Mathf.PI / 2.0f - Mathf.Asin(fSinLat * fSinDelta - fCosLat * fCosDelta * fCosT);
        float fPhi = Mathf.Atan((-fCosDelta * fSinT) / (fCosLat * fSinDelta - fSinLat * fCosDelta * fCosT));

        float opp = -fCosDelta * fSinT;
        float adj = -(fCosLat * fSinDelta + fSinLat * fCosDelta * fCosT);
        SolarAzimuth = Mathf.Atan2(opp, adj);
        solarAltitude = Mathf.Asin(fSinLat * fSinDelta - fCosLat * fCosDelta * fCosT);

        SetPosition(fTheta, fPhi);
    }
    
    Vector3 calcDirection(float thetaSun, float phiSun)
    {
        Vector3 dir = new Vector3();
        dir.x = Mathf.Cos(0.5f * Mathf.PI - thetaSun) * Mathf.Cos(phiSun);
        dir.y = Mathf.Sin(0.5f * Mathf.PI - thetaSun);
        dir.z = Mathf.Cos(0.5f * Mathf.PI - thetaSun) * Mathf.Sin(phiSun);
        return dir.normalized;
    }
    Vector3 sphericalToCartesian(Vector3 sunDir)
    {
        Vector3 res = new Vector3();
        res.y = sunDir.x * Mathf.Sin(sunDir.z);
        float tmp = sunDir.x * Mathf.Cos(sunDir.z);
        res.x = tmp * Mathf.Cos(sunDir.y);
        res.z = tmp * Mathf.Sin(sunDir.y);
        return res;
    }
    void ComputeAttenuation()
    {
        float fBeta = 0.04608365822050f * Turbidity - 0.04586025928522f;
        float fTauR, fTauA;
        float[] fTau = new float[3];
        float tmp = 93.885f - (m_fTheta / Mathf.PI * 180.0f);
        float m = (float)(1.0f / (Mathf.Cos(m_fTheta) + 0.15f * tmp));  // Relative Optical Mass
        float[] fLambda = new float[3];
        fLambda[0] = 0.65f;	// red (in um.)
        fLambda[1] = 0.57f;	// green (in um.)
        fLambda[2] = 0.475f;	// blue (in um.)

        for (int i = 0; i < 3; i++)
        {
            // Rayleigh Scattering
            // Results agree with the graph (pg 115, MI) */
            // lambda in um.
            fTauR = Mathf.Exp(-m * 0.008735f * Mathf.Pow(fLambda[i], -4.08f));

            // Aerosal (water + dust) attenuation
            // beta - amount of aerosols present 
            // alpha - ratio of small to large particle sizes. (0:4,usually 1.3)
            // Results agree with the graph (pg 121, MI) 
            const float fAlpha = 1.3f;
            if (m < 0.0f)
            {
                fTau[i] = 0.0f;
            }
            else
            {
                fTauA = Mathf.Exp(-m * fBeta * Mathf.Pow(fLambda[i], -fAlpha));  // lambda should be in um
                fTau[i] = fTauR * fTauA;
            }
        }

        RenderSettings.fogColor = new Color(fTau[0], fTau[1], fTau[2]);
        m_vColor = new Vector3(fTau[0], fTau[1], fTau[2]);
        sunLight.GetComponent<Light>().color = new Color(fTau[0], fTau[1], fTau[2]);
    }
    // end of sun calcs ----------------------------------------------------------------
}
