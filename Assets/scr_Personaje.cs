using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;


public class scr_Genoveva : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    BoxCollider2D box;
    public float fuerzaSalto;
    public bool enPiso;
    public Transform refPie;
    //Posicion Piso Mas bajo
    public Transform refPiso;
    public float velX;
    float movX;    
    //Particulas de sangre
    /* public GameObject particulasSangrePersonaje; */
    /* public UnityEngine.UI.Image mascaraDaño; */
    //Energia
    int energiaMax = 10;
    int energiaActual;
    /* public TMPro.TextMeshProUGUI textoEnergia; */
    /* public UnityEngine.UI.Image barraVida; */

    //Banderas Recogidas
    int banderasTotales = 5;
    int banderasRecogidas = 0;
    /* public TMPro.TextMeshProUGUI textoBanderas; */

    //Si el personaje muere
    /* public UnityEngine.UI.Image fondoMuerte; */
    float fondoMuerteDeseado;
    /* public TMPro.TextMeshProUGUI textoGameOver; */
    //Mision completa text
    /* public TMPro.TextMeshProUGUI textoMissionComplete; */
    //Variable para audio
    public AudioSource audioEscena;
    public AudioSource audioAuch;
    public AudioSource audioCaminar;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();     
        energiaActual = energiaMax;
        // Fade in Iniciar
        /* mascaraDaño.color = new Color(1, 1, 1, 1); //Negro */
        fondoMuerteDeseado = 0; //Transparente
        //Reanudar en caso haya una partida guardada
        if (infoPartida.partidaGuardada)
        {
            reanudarPartida();
        }
        audioEscena.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (energiaActual <= 0 || banderasRecogidas == banderasTotales)
        {
            return;
        }
        movX = Input.GetAxis("Horizontal");
        //Movimiento de caminata
        anim.SetFloat("absMovX", Mathf.Abs(movX));
        rb.velocity = new Vector2(velX * movX, rb.velocity.y);
        //Definir cuando esta tocando el piso para poder saltar sobre el 
        enPiso = Physics2D.OverlapCircle(refPie.position,0.5f,1<<3);
        //Llamar a la animacion 
        anim.SetBool("enPiso", enPiso);
        //if (Input.GetButtonDown("Jump")) anim.SetTrigger("Saltar");
        if (Input.GetButtonDown("Jump") && enPiso) rb.AddForce(new Vector2(0, fuerzaSalto), ForceMode2D.Impulse);
        //Girar personaje izq der
        if (movX < 0) {
            transform.localScale = new Vector3(-0.3f, 0.3f, 0f);
        }        
        if (movX > 0) {
            transform.localScale = new Vector3(0.3f, 0.3f, 0f);
        }   
        //Si la sbanderas recogidas son iguales al total de las banderas ejecutar partida finalizada

        if (transform.position.y < refPiso.position.y )
        {
            energiaActual = 0;
            GameOver();            
        }
        if (Mathf.Abs(movX) < 0.1 && enPiso)
        {
            audioCaminar.enabled = true;
        }
        else
        {
            audioCaminar.enabled = false;
        }
    }
    //Caer a presipicio o rio
    

    //Recibir golpe
    public void RecibirGolpe(Vector2 posicion)
    {
        // Reducir energia
        energiaActual -= 1;
        if (energiaActual <= 0)
        {
            GameOver();
            anim.SetTrigger("muere");              
        }
        else
        {
            
            float direccionGolpe = (transform.position.x-posicion[0]) / Mathf.Abs(transform.position.x-posicion[0]);
            Debug.Log(direccionGolpe.ToString());
            transform.position = new Vector3(transform.position.x + 1.5f * direccionGolpe, transform.position.y, 0f);
            anim.SetTrigger("auch");
            Debug.Log("Auch! ahora tengo " + energiaActual + " de " + energiaMax);
            // Punto en el cual aparecera la sangre
            /* Instantiate (particulasSangrePersonaje, posicion, Quaternion.identity);*/
        }
    }

    private void FixedUpdate() 
    {
        ActualizarDisplay();
        // FondoMuerte
        /* float valorAlfa = Mathf.Lerp(fondoMuerte.color.a, fondoMuerteDeseado, 0.1f);
        fondoMuerte.color = new Color(0, 0, 0, valorAlfa); */
        //Reiniciar escena
        // if (valorAlfa > 0.9f && fondoMuerteDeseado == 1)
        // {
        //     SceneManager.LoadScene("Scenes/EscapeGenoveva")
        // }

        if (banderasRecogidas == banderasTotales)        
        {
            partidaCompletada();
        }
    }

    private void OnCollisionStay2D(Collision2D collision) 
    {
        //colision con el protagonista
        if (collision.gameObject.CompareTag("Cactus"))
        {            
            RecibirGolpe(collision.contacts[0].point);                  
        } 
    } 

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("checkpoint"))
        {
            guardarPartida();
            Destroy(collision.gameObject);
            banderasRecogidas += 1;
            /* textoBanderas.text = banderasRecogidas.ToString() ; */
        }
    }
    
    void ActualizarDisplay() {
        //Porcentaje de energia
        float energiaPorcentual = energiaActual * 100 / energiaMax;
        /* textoEnergia.text = energiaPorcentual.ToString() + "%"; */
        //Disminucion de la barra de vida
        float energiaUnitaria =(float)energiaActual/energiaMax;
        /* barraVida.fillAmount = Mathf.Lerp(barraVida.fillAmount, energiaUnitaria, 0.1f); */

        //Mascara de daño
        float valorAlfa = 1 / (float)energiaMax * (energiaMax - energiaActual);
        /* mascaraDaño.color = new Color(1, 1, 1, valorAlfa); */
    }

    public void iniciarFadeOut() {
        fondoMuerteDeseado = 0.5f;        
    }

    void GameOver(){
        ActualizarDisplay();
        iniciarFadeOut();
        Debug.Log("Adios mundo cruel");                      
        /* textoGameOver.text = "GAME OVER"; */
        audioEscena.enabled = false;
    }

    public void partidaCompletada() {
        anim.SetTrigger("festejo");
        iniciarFadeOut();
        /* float valorAlfa = Mathf.Lerp(fondoMuerte.color.a, fondoMuerteDeseado, 0.1f); 
        fondoMuerte.color = new Color(0, 0, 0, valorAlfa); */
        /* textoMissionComplete.text = "MISSION COMPLETE"; */
        audioEscena.enabled = false;
    }

    public void guardarPartida() {
        infoPartida.infoPersonaje.energiaActual = energiaActual;
        infoPartida.infoPersonaje.posicion = transform.position;
        infoPartida.partidaGuardada = true;
    }
    
    public void reanudarPartida() {
        energiaActual = infoPartida.infoPersonaje.energiaActual;
        transform.position = infoPartida.infoPersonaje.posicion;
    }
}
