using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Necessário para reiniciar a cena e carregar o menu
using TMPro;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public float jumpForce = 5f; // Força de pulo
    public float rayDistance = 0.7f; // Distância do Raycast para detectar o chão

    // UI Elements
    public TextMeshProUGUI scoreText;
    public GameObject winTextObject;
    public TextMeshProUGUI timeText;
    public GameObject penaltyTextObject;
    public GameObject restartButton;
    public GameObject restartButtonBackground;

    // Configurações de jogo
    public float fallLimit = -10f;
    private Vector3 initialPosition;
    private Rigidbody rb;

    // Pontuação e movimentação
    private int score;
    private float movementX;
    private float movementY;

    // Temporizador
    private float timeRemaining = 90f;
    private bool timerActive;

    // Controle de movimentação
    private bool canMove;

    // Sons
    public AudioSource audioSource;
    public AudioClip pickUpSound;
    public AudioClip backgroundMusic;
    public AudioClip winSound;
    public AudioClip loseSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;

        initialPosition = transform.position;
        timerActive = true;
        canMove = true;

        // Inicializar UI
        SetScoreText();
        UpdateTimerDisplay();
        winTextObject.SetActive(false);
        restartButton.SetActive(false);
        restartButtonBackground.SetActive(false);
        penaltyTextObject.SetActive(false);

        // Tocar a música de fundo
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play(); // Toca a música de fundo
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
        // Verificar tecla de pulo
        if (canMove && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryJump();
        }

        // Atualizar cronômetro
        if (timerActive && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay(); // Atualiza a exibição do tempo
        }
        else if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame(false);  // Fim do jogo por tempo esgotado
        }

        // Verificar se a tecla "R" foi pressionada para reiniciar o jogo
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }

        // Verificar se a tecla "M" foi pressionada para voltar ao menu
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ReturnToMenu();
        }
    }

    void TryJump()
    {
        // Raycast para verificar se há chão com a tag "Jump"
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Jump"))
            {
                // Aplica a força de pulo
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        // Movimento do jogador
        if (canMove)
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * speed);
        }

        // Verificar se o jogador caiu do mapa
        if (transform.position.y < fallLimit)
        {
            RespawnPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            score += 1;

            // Tocar som de coleta
            if (audioSource != null && pickUpSound != null)
            {
                audioSource.PlayOneShot(pickUpSound);
            }

            SetScoreText();
        }

        // Colidir com inimigo estático
        if (other.gameObject.CompareTag("StaticEnemy"))
        {
            RespawnPlayer();
        }
    }

    // Atualiza o texto da pontuação na UI
    void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        // Verificar se o jogador atingiu a pontuação necessária para vencer
        if (score >= 22)
        {
            EndGame(true);  // Jogador ganhou
        }
    }

    // Atualiza a exibição do cronômetro na UI
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Reposiciona o jogador e aplica penalidade de tempo
    public void RespawnPlayer()
    {
        transform.position = initialPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Penalidade de tempo
        timeRemaining -= 5;

        // Mostrar texto de penalidade
        penaltyTextObject.SetActive(true);
        StartCoroutine(HidePenaltyTextAfterDelay(2f));
    }

    // Esconde o texto de penalidade após o atraso
    IEnumerator HidePenaltyTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        penaltyTextObject.SetActive(false);
    }

    // Função para terminar o jogo (vitória ou derrota)
    void EndGame(bool won)
    {
        if (!timerActive) return; // Evita que o jogo seja finalizado várias vezes

        timerActive = false;
        canMove = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Parar a música de fundo
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Tocar som de vitória ou derrota
        if (won)
        {
            winTextObject.SetActive(true);
            audioSource.PlayOneShot(winSound);
        }
        else
        {
            timeText.text = "Tempo esgotado! Fim de jogo!";
            audioSource.PlayOneShot(loseSound);
        }

        // Mostrar botão de reinício
        restartButton.SetActive(true);
        restartButtonBackground.SetActive(true);
    }

    // Reinicia o jogo, carregando a cena atual
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Retorna ao menu principal
    void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu"); // Certifique-se de que o nome da cena do menu principal seja "Menu"
    }
}
