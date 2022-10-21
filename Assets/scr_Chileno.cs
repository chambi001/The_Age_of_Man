using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Chileno : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    float limiteCaminataIzq;
    float limiteCaminataDer;
    public float velCaminata = 6f;
    int direccion = 1;
    public Transform refPie;
    enum tipoComportamiento {pasivo, persecucion, ataque}
    tipoComportamiento comportamiento = tipoComportamiento.pasivo;
    //Distancias a las zonas
    float distanciaPasiva = 7f;
    float distanciaPersecucion = 5f;
    public float distanciaAtaque = 0.47f;

    public float umbralVelocidad;
    float distanciaConPersonaje;
    public Transform Personaje;

    bool golpeValido = false;
    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        limiteCaminataIzq = transform.position.x - GetComponent<CircleCollider2D >().radius;
        limiteCaminataDer = transform.position.x + GetComponent<CircleCollider2D>().radius;     
        anim = transform.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        distanciaConPersonaje = Mathf.Abs(Personaje.position.x - transform.position.x);
        if(rb.velocity.magnitude < umbralVelocidad)
        {
            //Mover siempre que haya un layer piso a su alrededor
            bool enPiso = Physics2D.OverlapCircle(refPie.position,0.5f,1<<3);            
            switch (comportamiento)
            {
                case tipoComportamiento.pasivo://estando en la zona pasiva
                    //Desplazarse caminando
                    rb.velocity = new Vector2(velCaminata * direccion, rb.velocity.y);

                    //Para girar segun la zona de caminata
                    if (transform.position.x < limiteCaminataIzq) direccion = 1;
                    if (transform.position.x > limiteCaminataDer) direccion = -1;

                    //Aumneto en la velocidad del animator
                    anim.speed = 1f;

                    //Entrar a la zona de persecucion
                    if (distanciaConPersonaje < distanciaPersecucion) comportamiento = tipoComportamiento.persecucion;                    
           
                    break;
                case tipoComportamiento.persecucion://estandp en la Zona de persecucion
                    //Desplazarse Corriendo
                    rb.velocity = new Vector2(velCaminata * 1.2f * direccion, rb.velocity.y);

                    //Aumneto en la velocidad del animator
                    anim.speed = 1.2f;

                    //Para girar segun la Posicion del personaje
                    if (Personaje.position.x > transform.position.x) direccion = 1;
                    if (Personaje.position.x < transform.position.x) direccion = -1;

                    //Volver a la Zona pasiva
                    if (distanciaConPersonaje > distanciaPasiva) comportamiento = tipoComportamiento.pasivo;
                    //Entrar a Zona de ataque
                    if (distanciaConPersonaje < distanciaAtaque) comportamiento = tipoComportamiento.ataque;                
                    break;
                case tipoComportamiento.ataque://estando en la Zona de ataque              
                    //Lanzar el animator  
                    rb.velocity = new Vector2(velCaminata * 1.2f * direccion, rb.velocity.y);                   
                    anim.SetTrigger("atacar");
                    
                    //Para girar segun la Posicion del personaje
                    if (Personaje.position.x > transform.position.x) direccion = 1;
                    if (Personaje.position.x < transform.position.x) direccion = -1;

                    //Aumneto en la velocidad del animator
                    anim.speed = 1f;

                    //Entrar a Zona de persecucion
                    if (distanciaConPersonaje > distanciaAtaque) {
                        comportamiento = tipoComportamiento.persecucion;
                        anim.ResetTrigger("atacar");
                    }    
                    break;
            }
            //Poner en 0 la veloccidad o detener 
            if (!enPiso) 
            {
                rb.velocity = new Vector3(0, rb.velocity.y);
            }
        }
        transform.localScale = new Vector3(0.3f * -direccion,0.32f,0f);
    }
    public void golpeValido_inicio(){golpeValido = true;}
    public void golpeValido_fin(){golpeValido = false;}
    
    private void OnCollisionStay2D(Collision2D collision) 
    {
        //colision con el protagonista
        if (collision.gameObject.CompareTag("Player") && golpeValido)
        {
            golpeValido = false;
            Personaje.GetComponent<src_Personaje1>().RecibirGolpe(collision.contacts[0].point); 
            //Debug.Log(collision.contacts[0].point);            
        } 
        //Colision con objetos
        while (collision.gameObject.CompareTag("Objetos"))
        {            
            anim.SetTrigger("amenazar");                        
        } 
        anim.ResetTrigger("amenazar");
    }    
    
}
