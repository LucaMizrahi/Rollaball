using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;

    public TextMeshProUGUI scoreText;
    public GameObject winTextObject;
    public TextMeshProUGUI timeText; // Usar o componente de texto para exibir o tempo
    public GameObject restartButton;
    public GameObject restartButtonBackground;

    public float fallLimit = -10f;
    private Vector3 initialPosition;

    private Rigidbody rb;

    private int score;
    private float movementX;
    private float movementY;

    // Timer variables
    private float timeRemaining; // Armazena o tempo restante
    private bool timerActive;

    // Controla se o jogador pode se mover
    private bool canMove;

    // Tempo limite inicial (60 segundos)
    public float timeLimit = 60f;

    // Componente de som
    public AudioSource audioSource;
    // public AudioSource audioSourcePickUp;

    // Som para captura do bloco
    public AudioClip pickUpSound;

    // Música de fundo
    public AudioClip backgroundMusic;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;

        initialPosition = transform.position;

        // Inicializa o cronômetro com o valor de tempo limite
        timeRemaining = timeLimit;
        timerActive = true;

        // Inicialmente o jogador pode se mover
        canMove = true;

        SetScoreText();

        winTextObject.SetActive(false);
        restartButton.SetActive(false);
        restartButtonBackground.SetActive(false);

        // Tocar a música de fundo se estiver configurada
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;  // Faz a música de fundo tocar em loop
            audioSource.Play();       // Toca a música de fundo
        }
    }

    void OnMove(InputValue value)
    {
        // Só permite movimentação se o jogador puder se mover
        if (canMove)
        {
            Vector2 movementVector = value.Get<Vector2>();
            movementX = movementVector.x;
            movementY = movementVector.y;
        }
    }

    void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        if (score >= 13)
        {
            EndGame(true);  // Jogador ganhou o jogo
        }
    }

    void FixedUpdate() 
    {
        // Verifica se o jogador ainda pode se mover antes de aplicar movimento
        if (canMove)
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * speed);
        }

        if (transform.position.y < fallLimit)
        {
            RespawnPlayer();
        }

        // Atualizar o cronômetro enquanto o jogo estiver ativo
        if (timerActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;  // Reduz o tempo restante

                // Calcula os minutos e segundos restantes
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);

                // Exibir o tempo no formato "00:00" (minutos:segundos)
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                // Se o tempo acabar, terminar o jogo
                timeRemaining = 0;
                EndGame(false);  // O tempo acabou, o jogador perdeu
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            score = score + 1;

            // Toca o som de captura
            if (audioSource != null && pickUpSound != null)
            {
                audioSource.PlayOneShot(pickUpSound);
            }
            
            SetScoreText();
        }
    }

    // Função para reposicionar o jogador na posição inicial
    void RespawnPlayer()
    {
        transform.position = initialPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Debug.Log("Jogador caiu do mapa e foi reposicionado!");
    }

    // Função para encerrar o jogo
    void EndGame(bool won)
    {
        timerActive = false; // Para o cronômetro
        canMove = false;     // Impede o jogador de se mover

        // Zera a velocidade da bola ao fim do jogo
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Para a música de fundo quando o jogo termina
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (won)
        {
            winTextObject.SetActive(true);
            timeText.text = timeText.text;
        }
        else
        {
            // Exibir mensagem de tempo esgotado
            timeText.text = "Tempo esgotado! Fim de jogo!";
        }

        restartButton.SetActive(true); // Ativar botão de reinício
        restartButtonBackground.SetActive(true); // Ativar fundo do botão de reinício
    }
}
