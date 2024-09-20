using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public float jumpForce = 5f; // Força de pulo
    public float rayDistance = 0.7f; // Distância do Raycast para detectar o chão

    public TextMeshProUGUI scoreText;
    public GameObject winTextObject;
    public TextMeshProUGUI timeText;

    public GameObject penaltyTextObject;
    public GameObject restartButton;
    public GameObject restartButtonBackground;

    public float fallLimit = -10f;
    private Vector3 initialPosition;

    private Rigidbody rb;

    private int score;
    private float movementX;
    private float movementY;

    // Timer variables
    private float timeRemaining;
    private bool timerActive;

    // Controla se o jogador pode se mover
    private bool canMove;

    // Tempo limite inicial (60 segundos)
    public float timeLimit = 60f;

    // Componente de som
    public AudioSource audioSource;

    // Som para captura do bloco
    public AudioClip pickUpSound;

    // Música de fundo
    public AudioClip backgroundMusic;

    // Som para vitória
    public AudioClip winSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;

        initialPosition = transform.position;

        // Inicializa o cronômetro com o valor de tempo limite
        timeRemaining = timeLimit;
        timerActive = true;

        canMove = true;

        SetScoreText();

        winTextObject.SetActive(false);
        restartButton.SetActive(false);
        restartButtonBackground.SetActive(false);
        penaltyTextObject.SetActive(false);

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
        if (canMove)
        {
            Vector2 movementVector = value.Get<Vector2>();
            movementX = movementVector.x;
            movementY = movementVector.y;
        }
    }

    void Update()
    {
        // Verifica se a tecla espaço foi pressionada para pular
        if (canMove && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryJump();
        }
    }

    void TryJump()
    {
        // Faz um Raycast para verificar se há chão com a tag "jump" abaixo da bola
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Jump"))
            {
                // Aplica a força de pulo se a bola estiver no chão
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        if (score >= 12)
        {
            EndGame(true);  // Jogador ganhou o jogo
        }
    }

    void FixedUpdate() 
    {
        if (canMove)
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * speed);
        }

        if (transform.position.y < fallLimit)
        {
            RespawnPlayer();
        }

        if (timerActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);

                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                timeRemaining = 0;
                EndGame(false);
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

        if (other.gameObject.CompareTag("StaticEnemy"))
        {
            RespawnPlayer();
            // Diminui o tempo em 5 segundos ao colidir com o inimigo
            timeRemaining -= 5;
            // Ativar o objeto de texto de penalidade
            penaltyTextObject.SetActive(true);

            // Iniciar a corrotina para esconder o texto após 5 segundos
            StartCoroutine(HidePenaltyTextAfterDelay(2f));
        }
    }

    IEnumerator HidePenaltyTextAfterDelay(float delay)
    {
        // Espera por 'delay' segundos antes de esconder o texto
        yield return new WaitForSeconds(delay);
        penaltyTextObject.SetActive(false); // Esconder o texto de penalidade
    }

    void RespawnPlayer()
    {
        transform.position = initialPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Debug.Log("Jogador caiu do mapa e foi reposicionado!");
    }

    void EndGame(bool won)
    {
        timerActive = false;
        canMove = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Parar a música de fundo quando o jogo acabar
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (won)
        {
            winTextObject.SetActive(true);
            timeText.text = timeText.text;
            audioSource.PlayOneShot(winSound);
        }
        else
        {
            timeText.text = "Tempo esgotado! Fim de jogo!";
        }

        restartButton.SetActive(true);
        restartButtonBackground.SetActive(true);
    }
}
