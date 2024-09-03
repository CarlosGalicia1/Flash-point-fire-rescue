using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto con el que colisiona NO tiene el tag "Smoke"
        if (other.gameObject.tag != "Smoke")
        {
            // Destruye el objeto QuestionMark
            Destroy(gameObject);

            Debug.Log("QuestionMark destruido tras entrar en contacto con " + other.gameObject.name);
        }
    }
}
