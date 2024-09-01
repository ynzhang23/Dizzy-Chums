using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;

    public GameObject uI_3D_GameObject;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;
    
    private Rigidbody rb; 
    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;

    [Header("Player Type Damage Coefficients")]
    public float doDamageCoefficientAttacker = 10f;
    public float getDamageCoefficientAttacker = 1.2f;

    public float doDamageCoefficientDefender = 0.75f;
    public float getDamageCoefficientDefender = 0.2f;


    public float commonDamageCoefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;
    private bool isDead;

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;

            // Attacker has default 3600

        } else if (gameObject.name.Contains("Defender"))
        {
            isAttacker = false;
            isDefender = true;

            spinnerScript.spinSpeed = 4400;
            
            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedRatio_Text.text = currentSpinSpeed + " /" + startSpinSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("My speed " + mySpeed + " vs. Other speed: " + otherPlayerSpeed);

            if (mySpeed > otherPlayerSpeed)
            {
                Debug.Log("I damage other player");
                
                // Default damage standardised (based on current speed)
                float defaultDamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600 * commonDamageCoefficient;

                if (isAttacker) {
                    // Apply attacker multiplier
                    defaultDamageAmount *= doDamageCoefficientAttacker;
                } else if (isDefender) {
                    // Apply defender multiplier
                    defaultDamageAmount *= doDamageCoefficientDefender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine) {
                    //Apply Damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, defaultDamageAmount);
                }
            }
        }
    }

    [PunRPC]
    public void DoDamage(float _damageAmount)
    {
        if (!isDead) 
        {

            // Evaluate the damage based on player type
            if (isAttacker) 
            {
                // Attacker gets more damaged
                _damageAmount *= getDamageCoefficientAttacker;

                if (_damageAmount > 1000)
                {
                    _damageAmount = 400;
                }

            } else if (isDefender) 
            {
                // Defender gets more damaged
                _damageAmount *= getDamageCoefficientDefender;
            }

            spinnerScript.spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + " /" + startSpinSpeed;

            if (currentSpinSpeed < 100) 
            {
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;
        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 10f;

        uI_3D_GameObject.SetActive(false);

        if (photonView.IsMine)
        {
            //countdown for respawn
            StartCoroutine(Respawn());
        }

        IEnumerator Respawn()
        {
            GameObject canvasGameobject = GameObject.Find("Canvas");
            if (deathPanelUIGameobject == null)
            {
                deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameobject.transform);
            } 
            else
            {
                deathPanelUIGameobject.SetActive(true);
            }

            Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();
            
            float respawnTime = 8.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            while (respawnTime > 0.0f)
            {
                yield return new WaitForSeconds(1.0f);
                respawnTime -= 1.0f;
                respawnTimeText.text = respawnTime.ToString(".00");

                GetComponent<MovementController>().enabled = false;
            }

            deathPanelUIGameobject.SetActive(false);    

            GetComponent<MovementController>().enabled = true;

            photonView.RPC("ReBorn", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void ReBorn() {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = startSpinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + " /" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        uI_3D_GameObject.SetActive(true);
        isDead = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
