using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public float moveSpeed = 3f; // Velocidade de movimento
    public float detectionRange = 10f; // Distância de detecção
    // public Vector3 mapCenter; // Centro do região de movimento

    private Rigidbody rb;
    private PlayerController playerController; // Referência ao script do jogador

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = player.GetComponent<PlayerController>();        
    }

    // Update is called once per frame
    void Update()
    {
        // Distância entre o inimigo e o jogador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

         // Se a distância for menor ou igual que a distância de detecção
        if (distanceToPlayer <= detectionRange) {

            // Direção do jogador que o inimigo deve se mover
            Vector3 direction = (player.position - transform.position).normalized;

            // Novo vetor de direção do inimigo
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.z);

            // Define a velocidade de movimento do inimigo
            rb.velocity = moveDirection * moveSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision) 
    {
        // Se o inimigo colidir com o jogador
        if (collision.gameObject.CompareTag("Player")) {

            // Se o playerController não for nulo
            if (playerController != null) {

                // Chama o método de respawn do jogador
                playerController.RespawnPlayer();
            }
        }
    }

    // Método para desenhar o raio de detecção do inimigo
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
